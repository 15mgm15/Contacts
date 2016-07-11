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
using Contacts.Models;

namespace Contacts
{
    public class ListAdapter : BaseAdapter<Contact>
    {
        List<Contact> items;
        Activity context;
        public ListAdapter(Activity context, List<Contact> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Contact this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.cell, null);
            }
            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.Name + " " + item.LastName;
            view.FindViewById<TextView>(Resource.Id.Image).Text = item.Initials;
            return view;
        }
    }
}