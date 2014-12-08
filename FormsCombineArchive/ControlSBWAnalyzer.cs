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
using SBW;
using SBW.Utils;

namespace FormsCombineArchive
{
    public partial class ControlSBWAnalyzer : UserControl
    {
        private SortedList<string,ServiceDescriptor> _Analyzers;
        SBWFavorites favs;
        public bool IsAvailable { get; private set; }
        public Entry Current { get; set; }

        public string SBML
        {
            get
            {
                if (Current == null)
                    return "";
                return Current.GetContents();
            }
        }

        public ControlSBWAnalyzer()
        {
            InitializeComponent();
            IsAvailable = true;
        }

        private void OnGoClicked(object sender, EventArgs e)
        {
            try
            {
                var isConnected = SBWLowLevel.isConnected();
                if (!isConnected) SBWLowLevel.connect();
                var current = _Analyzers[(string)cmbAnalyzer.SelectedItem];
                HighLevel.Send(current.ModuleName, current.Name, "void doAnalysis(string)", SBML);
                if (!isConnected) SBWLowLevel.disconnect();
            }
            catch 
            {
                
            }
        }

        private void OnEditVisuallyClicked(object sender, EventArgs e)
        {
            favs.EditVisually();
        }

        private void OnEditScriptClicked(object sender, EventArgs e)
        {
            favs.EditScript();
        }

        private void OnSimulateClicked(object sender, EventArgs e)
        {
            favs.Simulate();
        }

        private void OnTranslateClicked(object sender, EventArgs e)
        {
            favs.Translate();
        }

        private void ControlSBWAnalyzer_Load(object sender, EventArgs e)
        {
            try
            {
                _Analyzers = SBWMenu.GetSortedAnalyzers();
            }
            catch
            {
                
            }

            

            if (_Analyzers == null || _Analyzers.Count == 0)
            {
                IsAvailable = false;
                Visible = false;
            }
            else
            {
                IsAvailable = true;
                cmbAnalyzer.Items.AddRange(_Analyzers.Keys.ToArray<string>());
                cmbAnalyzer.SelectedItem = cmbAnalyzer.Items[0];
                favs = new SBWFavorites(() => SBML);
            }
        }
    }
}
