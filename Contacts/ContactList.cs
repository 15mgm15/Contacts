using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Content.PM;
using Android.Views;
using Contacts.Models;
using System.Collections.Generic;
using System;

namespace Contacts
{
    [Activity(Label = "ContactList",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class ContactList : AppCompatActivity
    {
        List<Contact> _contactsList;
        Dialog _dialog;
        Contact _contactSelected;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.ContactListView);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "Contacts List";
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            ProgressDialog mDialog = new ProgressDialog(this);
            mDialog.SetMessage("Please wait...");
            mDialog.SetCancelable(false);

            RunOnUiThread(() =>
            {
                mDialog.Show();
            });

            //Task.Run( async () => {
            var contactsAdapter = new ContactsAdapter(this);
            contactsAdapter.FillContacts();
            _contactsList = contactsAdapter.ContactList;
            var contactsListView = FindViewById<ListView>(Resource.Id.ContactsListView);
            contactsListView.Adapter = contactsAdapter;
            contactsListView.ItemClick += OnListItemClick;

            RunOnUiThread(() =>
            {
                mDialog.Dismiss();
            });
            //});

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        #region Events

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            _contactSelected = _contactsList[e.Position];

            Console.WriteLine(_contactSelected);
            CreateDialog();
            _dialog.Show();

        }

        void CreateDialog()
        {
            _dialog = new Dialog(this);
            _dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            _dialog.SetContentView(Resource.Layout.custom_dialog_layout);
            Button addBtn = (Button)_dialog.FindViewById(Resource.Id.dialog_button);
            addBtn.Text = "Add Contact";
            addBtn.Click += AddButton_Click;

            TextView initials = (TextView)_dialog.FindViewById(Resource.Id.initialsDialog);
            initials.Text = _contactSelected.Initials;
            EditText name = (EditText)_dialog.FindViewById(Resource.Id.firstNameTxt);
            name.Text = _contactSelected.Name;
            EditText lastName = (EditText)_dialog.FindViewById(Resource.Id.lastNameTxt);
            lastName.Text = _contactSelected.LastName;
            EditText phone = (EditText)_dialog.FindViewById(Resource.Id.phoneNumberTxt);
            phone.Text = _contactSelected.Phone;
            EditText email = (EditText)_dialog.FindViewById(Resource.Id.emailTxt);
            email.Text = _contactSelected.Email;

        }

        void AddButton_Click(object sender, EventArgs e)
        {
            EditText name = (EditText)_dialog.FindViewById(Resource.Id.firstNameTxt);
            EditText lastName = (EditText)_dialog.FindViewById(Resource.Id.lastNameTxt);
            EditText phone = (EditText)_dialog.FindViewById(Resource.Id.phoneNumberTxt);
            EditText email = (EditText)_dialog.FindViewById(Resource.Id.emailTxt);

            int isValid = MainActivity.ValidateData(name.Text, lastName.Text, phone.Text, email.Text);

            if (isValid != 0)
            {
                switch (isValid)
                {
                    case 1:
                        Toast.MakeText(this, "Information Missing!", ToastLength.Short).Show();
                        break;
                    case 2:
                        Toast.MakeText(this, "Invalid Email!", ToastLength.Short).Show();
                        break;
                    case 3:
                        Toast.MakeText(this, "Invalid Phone!", ToastLength.Short).Show();
                        break;
                    default:
                        break;
                }
                return;
            }

            MainActivity.ContactList.Add(new Contact() { Name = name.Text, LastName = lastName.Text, Phone = phone.Text, Email = email.Text });
            _dialog.Dismiss();
            Finish();
        }

        public static int ValidateData(string name, string lastName, string phone, string email)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastName))
            {
                return 1;
            }
            else if (!MainActivity.IsEmailValid(email))
            {
                return 2;
            }
            else if (MainActivity.IsPhoneValid(phone))
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}