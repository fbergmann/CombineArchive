using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibCombine;

namespace FormsCombineArchive
{
    public partial class ControlCreator : UserControl
    {
        public ControlCreator()
        {
            InitializeComponent();
        }

        public string FirstName
        {
            get
            {
                return txtFirst.Text;
            }
            set
            {
                txtFirst.Text = value;
            }
        }
        public string LastName
        {
            get
            {
                return txtLast.Text;
            }
            set
            {
                txtLast.Text = value;
            }
        }

        public string Email
        {
            get
            {
                return txtEmail.Text;
            }
            set
            {
                txtEmail.Text = value;
            }
        }

        public string Organization
        {
            get
            {
                return txtOrg.Text;
            }
            set
            {
                txtOrg.Text = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public VCard VCard {
            get
            {
                return new VCard { 
                    GivenName = FirstName, FamilyName = LastName, Email = Email,
                    Organization = Organization };
            }
            set
            {
            	FirstName = value.GivenName;
            	LastName = value.FamilyName;
            	Email = value.Email;
            	Organization = value.Organization;
            }
        } 

    }
}
