using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonModel
{

    public class ComboxItem
    {
        private string text;
        private string values;

        public ComboxItem(string _Text, string _Values)
        {
            this.Text = _Text;
            this.Values = _Values;
        }

        public override string ToString()
        {
            return this.Text;
        }

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        public string Values
        {
            get
            {
                return this.values;
            }
            set
            {
                this.values = value;
            }
        }
    }
}
