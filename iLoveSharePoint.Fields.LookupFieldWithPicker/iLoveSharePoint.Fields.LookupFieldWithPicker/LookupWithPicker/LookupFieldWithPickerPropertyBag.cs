using System;
using System.Collections.Generic;
using System.Text;

namespace iLoveSharePoint.Fields
{
    public class LookupFieldWithPickerPropertyBag
    {
        public Guid ListId { get; set; }
        public Guid FieldId { get; set; }
        public int MaxSearchResults { get; set; }
        public int EntityEditorRows { get; set; }
        public Guid WebId { get; set; }

        private string _searchFields = "";

        public List<string> SearchFields
        {
            get 
            {
                return new List<string>(_searchFields.Split(',')); 
            }
            set 
            {
                string str = "";
                foreach (string strField in value)
                {
                    if (str.Length > 0)
                        str += ",";
                    str += strField;
                }

                _searchFields = str; 
            }
        }

        public LookupFieldWithPickerPropertyBag()
        { 
        }

        public LookupFieldWithPickerPropertyBag(string value)
        {
            string[] tokens = value.Split(';');
            this.ListId = new Guid(tokens[0]);
            this.FieldId = new Guid(tokens[1]);
            this._searchFields = tokens[2];
            this.MaxSearchResults = int.Parse(tokens[3]);
            this.EntityEditorRows = int.Parse(tokens[4]);
            this.WebId = new Guid(tokens[5]);
        }

        public LookupFieldWithPickerPropertyBag(Guid webId, Guid listId, Guid fieldId,List<string> searchFields, int maxSearchResults, int entityEditorRows)
        {
            this.ListId = listId;
            this.FieldId = fieldId;
            this.SearchFields = searchFields;
            this.MaxSearchResults = maxSearchResults;
            this.EntityEditorRows = entityEditorRows;
            this.WebId = webId;
        }

        public override string ToString()
        {
            return ListId.ToString() + ";" + FieldId.ToString() + ";" + _searchFields + ";" + MaxSearchResults +  ";" + EntityEditorRows + ";" + WebId.ToString();
        }
    }
}
