using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LibCombine
{
    /// <summary>
    /// Class representing the VCard element in the Omex Description
    /// </summary>
    public class VCard
    {
        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        /// <value>
        /// The last name 
        /// </value>
        public string FamilyName { get; set; }
        /// <summary>
        /// Gets or sets the firstname
        /// </summary>
        /// <value>
        /// The first name
        /// </value>
        public string GivenName { get; set; }
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>
        /// The organization.
        /// </value>
        public string Organization { get; set; }

        public bool Empty
        {
            get
            {
                return !string.IsNullOrWhiteSpace(FamilyName) || !string.IsNullOrWhiteSpace(GivenName);
            }
        }
        public string ToXML()
        {
            if (Empty) return "";

            const string format = @"
              <dcterms:creator>
                <rdf:Bag>
                  <rdf:li rdf:parseType='Resource'>
                    <vCard:n rdf:parseType='Resource'>
                      <vCard:family-name>{0}</vCard:family-name>
                      <vCard:given-name>{1}</vCard:given-name>
                    </vCard:n>
                    <vCard:email>{2}</vCard:email>
                    <vCard:org rdf:parseType='Resource'>
                      <vCard:organization-name>{3}</vCard:organization-name>
                    </vCard:org>
                  </rdf:li>
                </rdf:Bag>
              </dcterms:creator>";
            return string.Format(format, FamilyName, GivenName, Email, Organization);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VCard"/> class.
        /// </summary>
        public VCard() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="VCard"/> class.
        /// </summary>
        /// <param name="element">The element to read from.</param>
        public VCard(XmlElement element)
            : this()
        {
            const string vcardNS = "http://www.w3.org/2006/vcard/ns#";
            var list = element.GetElementsByTagName("family-name", vcardNS);
            if (list.Count > 0) FamilyName = list[0].InnerText;
            list = element.GetElementsByTagName("given-name", vcardNS);
            if (list.Count > 0) GivenName = list[0].InnerText;
            list = element.GetElementsByTagName("email", vcardNS);
            if (list.Count > 0) Email = list[0].InnerText;
            list = element.GetElementsByTagName("organization-name", vcardNS);
            if (list.Count > 0) Organization = list[0].InnerText;
            
        }

    }
}
