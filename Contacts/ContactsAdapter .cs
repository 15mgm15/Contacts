using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Contacts.Models;
using Android.Provider;
using Android.Content;
using Android.Database;
using System.Linq;
using System.Text;

namespace Contacts
{
    public class ContactsAdapter : BaseAdapter
    {
        public List<Contact> ContactList;
        Activity activity;

        public ContactsAdapter(Activity activity)
        {
            this.activity = activity;
            //FillContacts();
        }

        public override int Count
        {
            get { return ContactList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return ContactList[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.ContactListItem, parent, false);
            var contactName = view.FindViewById<TextView>(Resource.Id.ContactName);

            contactName.Text = ContactList[position].Name + " "  + ContactList[position].LastName;
            var contactUri = ContentUris.WithAppendedId(ContactsContract.Contacts.ContentUri, ContactList[position].Id);

            return view;
        }

        public void FillContacts()
        {
            var uri = ContactsContract.Contacts.ContentUri;

            string[] projection = {
                ContactsContract.Contacts.InterfaceConsts.Id,
                ContactsContract.Contacts.InterfaceConsts.DisplayName
            };

            var loader = new CursorLoader(activity, uri, projection, null, null, null);
            var cursor = (ICursor)loader.LoadInBackground();
            ContactList = new List<Contact>();

            //StringBuilder phonesSb = new StringBuilder();
            string phonesSb = string.Empty;
            StringBuilder emailsSb = new StringBuilder();

            if (cursor.MoveToFirst())
            {
                do
                {
                    Console.WriteLine(projection);

                    var id = cursor.GetInt(cursor.GetColumnIndex(projection[0]));

                    string name = cursor.GetString(cursor.GetColumnIndex(projection[1]));
                    var completeName = name.Split(' ');
                    string lastName = completeName.Length > 1 ? completeName[1] : string.Empty;
                    name = completeName[0];

                    //Get Email
                    loader = new CursorLoader(activity, ContactsContract.CommonDataKinds.Email.ContentUri, null,
                    ContactsContract.CommonDataKinds.Email.InterfaceConsts.ContactId + " = " + id,
                    null, null);

                    var nestedCursor = (ICursor)loader.LoadInBackground();

                    if (nestedCursor != null)
                    {
                        if (nestedCursor.MoveToFirst())
                        {
                            do
                            {
                                emailsSb.Append(nestedCursor.GetString(
                                    nestedCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data)));
                            } while (nestedCursor.MoveToNext());
                        }
                        else
                        {
                            Console.WriteLine("Contact does not have email!");
                        }
                    }

                    //GetPhone
                    loader = new CursorLoader(activity, ContactsContract.CommonDataKinds.Phone.ContentUri, null,
                    ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = " + id,
                    null, null);
                    nestedCursor = (ICursor)loader.LoadInBackground();

                    if (nestedCursor != null)
                    {
                        if (nestedCursor.MoveToFirst())
                        {
                            //do
                            //{
                            phonesSb = nestedCursor.GetString(
                                        nestedCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Data));
                            //phonesSb.Append(nestedCursor.GetString(
                            //        nestedCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data)));
                            //} while (nestedCursor.MoveToNext());
                        }
                        else
                        {
                            Console.WriteLine("Contact does not have phone numbers!");
                        }
                    }

                    var existingContact = ContactList.FirstOrDefault(c => c.Name == name && c.LastName == lastName);

                    if (existingContact == null || string.IsNullOrEmpty(phonesSb) || string.IsNullOrEmpty(phonesSb))
                    {
                        if (existingContact == null)
                        {
                            var contact = new Contact
                            {
                                Id = id,
                                Name = name,
                                LastName = lastName,
                                Phone = phonesSb.ToString(),
                                Email = emailsSb.ToString()
                            };
                            ContactList.Add(contact);
                        }
                        else
                        {
                            existingContact.Phone = existingContact.Phone + phonesSb.ToString();
                            existingContact.Email = existingContact.Email + emailsSb.ToString();
                        }
                    }
                    emailsSb.Clear();
                    //phonesSb.Clear();
                    phonesSb = string.Empty;
                } while (cursor.MoveToNext());
            }

            //return true;
        }
    }
}


