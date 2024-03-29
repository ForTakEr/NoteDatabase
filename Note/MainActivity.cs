﻿using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;

namespace Note
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        DatabaseService databaseService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            databaseService = new DatabaseService();
            databaseService.CreateDatabase();

            var noteAddBtn = FindViewById<Button>(Resource.Id.addNoteBtn);
            noteAddBtn.Click += NoteAddBtn_Click;
        }

        protected override void OnPostResume()
        {
            base.OnPostResume();

            var notes = databaseService.GetAllNotes();
            var listview = FindViewById<ListView>(Resource.Id.listView1);
            listview.Adapter = new CustomAdapter(this, notes, databaseService);
        }

        private void NoteAddBtn_Click(object sender, System.EventArgs e)
        {
            Notes notes = new Notes();
            var titleInput = FindViewById<EditText>(Resource.Id.inputTitle);
            var contentInput = FindViewById<EditText>(Resource.Id.inputContent);

            notes.Title = titleInput.Text;
            notes.Content = contentInput.Text;

            databaseService.AddNote(notes);

            var note = databaseService.GetAllNotes();
            var listView = FindViewById<ListView>(Resource.Id.listView1);
            listView.Adapter = new CustomAdapter(this, note, databaseService);
        }
    }
}