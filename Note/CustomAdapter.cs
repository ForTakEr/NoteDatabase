using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Note
{
    class CustomAdapter : BaseAdapter<Notes>
    {
        List<Notes> items;
        Activity context;
        DatabaseService databaseService;

        public CustomAdapter(Activity context, List<Notes> items, DatabaseService databaseService) : base()
        {
            this.context = context;
            this.items = items;
            this.databaseService = databaseService;
        }

        public override Notes this[int position]
        {
            get { return items[position]; }
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.CustomRow, null);
            }

            var title = view.FindViewById<TextView>(Resource.Id.title);
            title.Text = items[position].Title;

            view.Tag = position;
            view.Click -= View_Click;
            view.Click += View_Click;
            return view;
        }

        private void View_Click(object sender, EventArgs e)
        {
            var position = (int)((View)sender).Tag;

            Intent intent = new Intent(context, typeof(NoteActivity));
            intent.PutExtra("note", JsonConvert.SerializeObject(items[position]));
            context.StartActivity(intent);
        }

        [Activity(Label = "NoteActivity")]
        private class NoteActivity : Activity
        {
            DatabaseService databaseService;
            Notes notes;

            protected override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);

                SetContentView(Resource.Layout.note_activity);

                notes = JsonConvert.DeserializeObject<Notes>(Intent.GetStringExtra("note"));
                databaseService = new DatabaseService();
                databaseService.CreateDatabase();

                var title = FindViewById<TextView>(Resource.Id.title);
                var content = FindViewById<TextView>(Resource.Id.content);
                var save = FindViewById<Button>(Resource.Id.save);
                var delete = FindViewById<Button>(Resource.Id.delete);

                title.Text = notes.Title;
                content.Text = notes.Content;

                save.Click += Save_Click;
                delete.Click += Delete_Click;
            }

            private void Save_Click(object sender, EventArgs e)
            {
                var content = FindViewById<TextView>(Resource.Id.content);

                notes.Content = content.Text;
                databaseService.UpdateNote(notes);
                Finish();
            }

            private void Delete_Click(object sender, EventArgs e)
            {
                databaseService.RemoveNote(notes);
                Finish();
            }
        }
    }
}