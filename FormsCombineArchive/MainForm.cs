using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FormsCombineArchive.Properties;
using LibCombine;

namespace FormsCombineArchive
{
  public partial class MainForm : Form
  {
    private ListViewGroup grpSBML;
    private ListViewGroup grpSEDML;
    private ListViewGroup grpSBGN;
    private ListViewGroup grpData;
    private ListViewGroup grpDoc;
    private ListViewGroup grpImages;
    private ListViewGroup grpOther;

    internal CombineArchive Archive { get; set; }

    private void SetupGroups()
    {
      var alignment = HorizontalAlignment.Center;
      grpSBML = new ListViewGroup("SBML Files", alignment) { Header = "SBML Files", HeaderAlignment = alignment, Name = "grpSBML" };
      grpSEDML = new ListViewGroup("SED-ML files", alignment) { Header = "SED-ML files", HeaderAlignment = alignment, Name = "grpSEDML" };
      grpSBGN = new ListViewGroup("SBGN files", alignment) { Header = "SBGN files", HeaderAlignment = alignment, Name = "grpSBGN" };
      grpData = new ListViewGroup("Data files", alignment) { Header = "Data files", HeaderAlignment = alignment, Name = "grpData" };
      grpDoc = new ListViewGroup("Documents", alignment) { Header = "Documents", HeaderAlignment = alignment, Name = "grpDocs" };
      grpImages = new ListViewGroup("Images", alignment) { Header = "Images", HeaderAlignment = alignment, Name = "grpImages" };
      grpOther = new ListViewGroup("Unsorted", alignment) { Header = "Unsorted", HeaderAlignment = alignment, Name = "lstOther" };
      lstEntries.Groups.Clear();
      lstEntries.Groups.AddRange(new[] { grpSBML, grpSEDML, grpData, grpDoc, grpImages, grpOther });
    }
    public MainForm()
    {
      InitializeComponent();

      SetupGroups();

      NewArchive();

      lblMessage.Text = "Version: " + AboutBox.AssemblyVersion;

    }

    private ListViewGroup GetGroupForFormat(string format)
    {
      if (Entry.IsFormat("sbml", format))
        return grpSBML;
      if (Entry.IsFormat("sedml", format))
        return grpSEDML;
      if (Entry.IsFormat("sbgn", format))
        return grpSBGN;
      if (Entry.IsFormat("csv", format) || Entry.IsFormat("numl", format))
        return grpData;
      if (Entry.IsFormat("pdf", format) ||
          format.Contains("text/"))
        return grpDoc;
      if (format.Contains("image/"))
        return grpImages;

      return grpOther;
    }
    private string GetNameForLocation(string location)
    {
      var index = location.LastIndexOfAny(new[] { '\\', '/' });
      return location.Substring(index + 1);
    }
    private string GetToolTip(Entry entry)
    {
      return entry.Location;
    }
    private void AddEntry(Entry entry)
    {
      if (entry == null)
        return;
      var item = new ListViewItem
      {
        Text = GetNameForLocation(entry.Location),
        Group = GetGroupForFormat(entry.Format),
        Tag = entry,
        ToolTipText = GetToolTip(entry)
      };

      foreach (var xref in entry.CrossReferences)
      {
        var xentry = Archive[xref];
        if (xentry == null)
          continue;

        var xitem = new ListViewItem.ListViewSubItem
        {
          Text = GetNameForLocation(xref),
          Tag = xentry,
           
        };
        item.SubItems.Add(xitem);
      }

      lstEntries.Items.Add(item);
    }
    private void UpdateUI()
    {
      lstEntries.Clear();
      Text = "COMBINE Archive";
      if (Archive == null || Archive.Entries == null)
        return;

      foreach (Entry entry in Archive.Entries)
      {
        AddEntry(entry);
      }

      Text = string.Format("COMBINE Archive - [{0}]", Path.GetFileName(Archive.ArchiveFileName));

      previewHandlerHost1.Visible = false;
      webBrowser1.Visible = false;
      pictureBox1.Visible = true;
      textPanel.Visible = false;
      pictureBox1.Visible = true;
      pictureBox1.Image = Resources.COMBINE_ARCHIVE;

    }

    public void OpenFile(string fileName)
    {
      try
      {
        Archive = CombineArchive.FromFile(fileName);
        UpdateUI();
      }
      catch (Exception ex)
      {
        MessageBox.Show("An error occurred while opening the file. The exception was: " + Environment.NewLine + Environment.NewLine + ex.Message, "Could not open archive", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void OnOpenClicked(object sender, EventArgs e)
    {
      using (var dialog = new OpenFileDialog { Filter = "OMEX files|*.omex;*.sedx;*.sbex;*.cmex;*.phex;*.neux;*.sbox|All files|*.*" })
      {
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          OpenFile(dialog.FileName);
        }
      }
    }

    public void NewArchive()
    {
      Archive = new CombineArchive();
      UpdateUI();
    }
    private void OnNewClicked(object sender, EventArgs e)
    {
      NewArchive();
    }

    public void SaveFile(string fileName)
    {
      try
      {
        Archive.SaveTo(fileName);
        Archive.ArchiveFileName = fileName;
        Text = string.Format("COMBINE Archive - [{0}]", Path.GetFileName(Archive.ArchiveFileName));
      }
      catch (Exception ex)
      {
        MessageBox.Show("An error occurred while saving the file. Please ensure that all files in this archive are closed, so they can be read. The exception was: " + Environment.NewLine + Environment.NewLine + ex.Message, "Could not save archive", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
      using (var dialog = new SaveFileDialog { Filter = "OMEX files|*.omex;*.sedx;*.sbex;*.cmex;*.phex;*.neux;*.sbox|All files|*.*" })
      {
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          SaveFile(dialog.FileName);
        }
      }
    }

    private void OnExitClicked(object sender, EventArgs e)
    {
      Close();
    }

    private void OnAboutClicked(object sender, EventArgs e)
    {
      using (var dialog = new AboutBox())
        dialog.ShowDialog();
    }

    private void OnCutClicked(object sender, EventArgs e)
    {

    }

    private void OnCopyClicked(object sender, EventArgs e)
    {
      if (ctrlPlainText.Focused)
        ctrlPlainText.Copy();
      else
        OnCopyItemToClipboard(sender, e);
    }

    private void OnPasteClicked(object sender, EventArgs e)
    {

    }

    private void OnSelectAllClicked(object sender, EventArgs e)
    {
      ctrlPlainText.SelectAll();
    }


    private void OnEntriesDoubleClicked(object sender, EventArgs e)
    {
      var item = lstEntries.FocusedItem;
      if (item == null || item.Tag == null)
        return;
      ((Entry)(item.Tag)).OpenLocation();
    }

    private void ShowImage(string filename)
    {
      previewHandlerHost1.Visible = false;
      webBrowser1.Visible = false;
      pictureBox1.Visible = true;
      textPanel.Visible = false;
      pictureBox1.ImageLocation = filename;
    }

    
    private void DisplayUrl(string location)
    {
      previewHandlerHost1.Visible = false;
      textPanel.Visible = false;
      pictureBox1.Visible = false;
      webBrowser1.Visible = true;

      webBrowser1.Navigate(location);
    }
    private void OnSelectionChanged(object sender, EventArgs e)
    {
      if (lstEntries.SelectedItems == null || lstEntries.SelectedItems.Count < 1)
      {
        previewHandlerHost1.Visible = false;
        webBrowser1.Visible = false;
        pictureBox1.Visible = true;
        textPanel.Visible = false;
        pictureBox1.Visible = true;
        pictureBox1.Image = Resources.COMBINE_ARCHIVE;
        return;
      }
      var item = lstEntries.SelectedItems[0];
      if (item == null || item.Tag == null)
        return;
      var entry = ((Entry)(item.Tag));
      if (entry == null)
        return;

      if (entry.Description != null && entry.Description.Description != null) lblMessage.Text = entry.Description.Description.Trim();

      if (Entry.IsFormat("sbml",entry.Format) && controlSBWAnalyzer1.IsAvailable)
      {

        controlSBWAnalyzer1.Visible = true;
        controlSBWAnalyzer1.Current = entry;
      }
      else
      {
        controlSBWAnalyzer1.Visible = false;
      }

      string filename = entry.GetLocalFileName();

      string location = entry.Location;
      if (location.Contains("http://"))
      {
        DisplayUrl(location);
        return;
      }

      if (filename == null)
        return;
      if (previewHandlerHost1.Open(filename))
      {
        textPanel.Visible = false;
        webBrowser1.Visible = false;
        pictureBox1.Visible = false;
        previewHandlerHost1.Visible = true;
      }
      else if (entry.Format == "text/html" || entry.Format == "text/xhtml" || entry.Format == "image/svg+xml")
      {
        DisplayUrl(filename);
      }
      else if (entry.Format.Contains("image/"))
      {
        ShowImage(filename);
      }
      else
      {
        previewHandlerHost1.Visible = false;
        webBrowser1.Visible = false;
        pictureBox1.Visible = false;
        textPanel.Visible = true;
        ctrlPlainText.InitializeFromFile(filename);
      }
    }

    public void AddFile(string filename = "")
        {
            var dialog = new FormAddFile { FileName = filename, Format = Entry.GuessFormat(filename) };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var entry = Archive.AddEntry(
                    dialog.FileName, 
                    dialog.Format, 
                    dialog.Description);
              
                AddEntry(entry);
            }
        }


    private void OnAddFileClicked(object sender, EventArgs e)
    {
      AddFile();
    }

    private void OnDragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        e.Effect = DragDropEffects.Copy;
        return;
      }
      e.Effect = DragDropEffects.None;
    }

    private void OnDragDrop(object sender, DragEventArgs e)
    {
      var sFilenames = (string[])e.Data.GetData(DataFormats.FileDrop);
      var oInfo = new FileInfo(sFilenames[0]);
      if (oInfo.Extension.IsOneOf( ".omex", ".sedx", ".sbex", ".cmex", ".neux", ".phex", ".sbox"))
      {
        OpenFile(sFilenames[0]);
      }
      else
      {
        foreach(var item in sFilenames )
        AddFile(item);
      }
    }

    public Entry GetCurrenEntry()
    {
      ListViewItem item;
      return GetCurrenEntry(out item);
    }
    public Entry GetCurrenEntry(out ListViewItem item)
    {
      item = null;
      if (lstEntries.SelectedItems == null || lstEntries.SelectedItems.Count < 1)
        return null;
      item = lstEntries.SelectedItems[0];
      if (item == null || item.Tag == null)
        return null;
      var entry = ((Entry)(item.Tag));
      if (entry == null)
        return null;
      return entry;
    }

    private void OnRemoveEntry(object sender, EventArgs e)
    {
      ListViewItem item; ;
      var entry = GetCurrenEntry(out item);
      if (entry == null) return;

      if (MessageBox.Show(string.Format("Do you really want to delete '{0}'?", item.Text), "Delete Entry", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
      {
        lstEntries.Items.Remove(item);
        Archive.Entries.Remove(entry);
      }
    }

    private void OnOpenItem(object sender, EventArgs e)
    {
      var entry = GetCurrenEntry();
      if (entry == null) return;
      entry.OpenLocation();
    }

    private void OnSaveItem(object sender, EventArgs e)
    {
      var entry = GetCurrenEntry();
      if (entry == null) return;
      var local = entry.GetLocalFileName();
      if (local == null) return;
      var ext = Path.GetExtension(local);

      try
      {
        using (var dialog = new SaveFileDialog { Filter = "*" + ext })
        {
          if (dialog.ShowDialog() == DialogResult.OK
              )
            File.Copy(local, dialog.FileName);
        }
      }
      catch 
      {

      }
    }

    private void OnDisplayMetaDataClick(object sender, EventArgs e)
    {
      ListViewItem item; ;
      var entry = GetCurrenEntry(out item);
      if (entry == null) return;

      OmexDescription desc = entry.Description;
      var dialog = new FormDisplayData { FileName = entry.Location, Format = entry.Format, Description = desc };
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        entry.Location = dialog.FileName;
        entry.Format = dialog.Format;
        Archive.Descriptions.Remove(desc);
        Archive.Descriptions.Add(dialog.Description);
      }
    }


    private void OnCopyItemToClipboard(object sender, EventArgs e)
    {
      ListViewItem item; ;
      var entry = GetCurrenEntry(out item);
      if (entry == null) return;
      var local = entry.GetLocalFileName();
      if (local == null) return;
      try
      {
        StringCollection col = new StringCollection();
        col.Add(local);
        Clipboard.SetFileDropList(col);
      }
      catch
      {

      }

    }

    private void OnCopyContentToClipboard(object sender, EventArgs e)
    {

      try
      {
        var entry = GetCurrenEntry();
        if (entry == null) return;
        Clipboard.SetText(entry.GetContents());
      }
      catch
      {

      }
    }

    private void OnHomePageButtonClicked(object sender, EventArgs e)
    {
      DisplayUrl("http://fbergmann.github.io/CombineArchive");
    }

    private void OnSubmitBugClicked(object sender, EventArgs e)
    {

      try
      {
        Process.Start("http://github.com/fbergmann/CombineArchive/issues");
      }
      catch
      {

      }

    }

    private void OnValidateClick(object sender, EventArgs e)
    {
      var errors = Archive.Validate();
      if (errors.Count == 0)
      {
        MessageBox.Show("No issues found.", "Archive is valid", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      var sb = new StringBuilder();
      foreach (var item in errors)
        sb.AppendFormat("{0}: {1}{2}", item.Item1, item.Item2, Environment.NewLine);

      if (errors.FirstOrDefault( t => t.Item1 == "error") != null)
      {
        MessageBox.Show(this, string.Format("Errors found: {0}{0}{1}", Environment.NewLine, sb.ToString()), "Archive is invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      MessageBox.Show(this, string.Format("Issues found: {0}{0}{1}", Environment.NewLine, sb.ToString()), "Archive has issues", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    }

    private void OnEditArchivePropertiesClick(object sender, EventArgs e)
    {
      OmexDescription desc = Archive.Descriptions.FirstOrDefault(d => d.About == ".");
      if (desc == null)
      {
        desc = new OmexDescription {About = ".", Created = DateTime.Now};
        Archive.Descriptions.Add(desc);
      }
      var dialog = new FormDisplayData { FileName = ".", Format = Entry.KnownFormats["omex"], Description = desc };
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        Archive.Descriptions.Remove(desc);
        Archive.Descriptions.Add(dialog.Description);
      }
    }
  }
}
