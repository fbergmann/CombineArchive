using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsCombineArchive
{
  public partial class ControlPlainTextEdit : UserControl
  {
    public ControlPlainTextEdit()
    {
      InitializeComponent();
      menuStrip1.Visible = false;
    }

    public string FileName { get; set; }

    public void Copy()
    {
      textBox1.Copy();
    }

    public void InitializeFromFile(string filename)
    {
      var info = new FileInfo(filename);
      if (info.Length > 1*1024*1024)
      {
        InitializeFrom("File is too big to be displayed");
      }
      else
      {
        InitializeFrom(File.ReadAllText(filename), true);
      }       
      FileName = filename;
    }

    public void InitializeFrom(string content, bool saveVisible = false)
    {
      FileName = null;

      textBox1.Text = content.Replace("\n", Environment.NewLine).Replace("\r\r", "\r");
      menuStrip1.Visible = saveVisible;
    }

    public void SelectAll()
    {
      textBox1.SelectAll();
    }
    private void OnSaveClick(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(FileName))
        return;
      File.WriteAllText(FileName, textBox1.Text);
    }

    private void OnReloadClick(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(FileName))
        return;
      InitializeFromFile(FileName);

    }
  }
}
