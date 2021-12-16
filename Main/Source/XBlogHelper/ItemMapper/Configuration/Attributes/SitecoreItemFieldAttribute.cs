﻿using System;
using Sitecore.Data;

namespace XBlogHelper.ItemMapper.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SitecoreItemFieldAttribute : Attribute
    {
        public SitecoreItemFieldAttribute(ID fieldID)
        {
            FieldId = fieldID.ToString();
        }

        public SitecoreItemFieldAttribute(string fieldID)
        {
            FieldId = fieldID;
        }

        public string FieldId { get; set; }
    }
}
