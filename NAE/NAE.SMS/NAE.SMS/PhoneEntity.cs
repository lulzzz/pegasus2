using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NAE.SMS
{
    public class PhoneEntity : TableEntity
    {
        private string _phone;

        public PhoneEntity()
        {
        }

        public PhoneEntity(string phone)
            : this(phone, false)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(phone));
                this.PartitionKey = new Guid(hash).ToString();
                this.Phone = phone;
            }
        }

        public PhoneEntity(string phone, bool activated)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(phone));
                this.Phone = phone;
                this.Activated = activated;
                this.PartitionKey = new Guid(hash).ToString();
                this.RowKey = phone;
            }
        }

        public string Phone
        {
            get
            {
                return this._phone;
            }
            set
            {
                this.RowKey = value;
                this._phone = value;
            }
        }
        public bool Activated { get; set; }
    }
}
