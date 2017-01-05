using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Azure.DocumentDBStudio.CustomDocumentListDisplay;
using Microsoft.Azure.DocumentDBStudio.Helpers;
using Microsoft.Azure.DocumentDBStudio.Properties;

namespace Microsoft.Azure.DocumentDBStudio
{
    public partial class CustomDocumentListDisplayConfigurationForm : Form
    {
        private readonly string _hostName;
        private readonly string _databaseId;
        private readonly string _documentCollectionId;
        private readonly dynamic _firstDocument;
        private List<string> _propertyKeysForDocument;
        private CustomDocumentListDisplay.Models.CustomDocumentListDisplay _setting;
        private readonly CustomDocumentListDisplayManager _customDocumentListDisplayManager = new CustomDocumentListDisplayManager();

        public CustomDocumentListDisplayConfigurationForm(string hostName, string databaseId, string documentCollectionId, dynamic firstDocument)
        {
            _hostName = hostName;
            _databaseId = databaseId;
            _documentCollectionId = documentCollectionId;
            _firstDocument = firstDocument;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void CustomDocumentListDisplayForm_Load(object sender, EventArgs e)
        {
            lblCannotSaveHint.Text = "";
            Text = "Configure Document Listing settings";
            tbDefaultGlobalFieldName.Text = Settings.Default.CustomDocumentDisplayIdentifier;

            gbCustom.Text = string.Format("Custom Document Listing settings for {0}/{1}", _databaseId, _documentCollectionId);

            _propertyKeysForDocument = DynamicHelper.GetPropertyKeysForDynamic(_firstDocument);
            foreach (var prop in _propertyKeysForDocument)
            {
                cbSortFields.Items.Add(prop);
            }

            _setting = _customDocumentListDisplayManager.FindCustomDocumentListDisplay(_hostName, _databaseId, _documentCollectionId);

            SetStatusForSettingFields(_setting != null);

            SetAllSettingFields();
        }

        private void SetAllSettingFields()
        {
            if (_setting == null) return;
            tbSortField.Text = _setting.SortBy;
            SetSortSelectedIndexByValue(_setting.SortBy);
            tbDisplayPattern.Text = _setting.DisplayPattern;
            cbReverseSort.Checked = _setting.ReverseSort;
            SetPatternExampleField();
        }

        private void CreateNewSetting()
        {
            _setting = new CustomDocumentListDisplay.Models.CustomDocumentListDisplay
            {
                DatabaseId = _databaseId,
                DocumentCollectionId = _documentCollectionId,
                SortBy = "id",
                DisplayPattern = "{id}"
            };

            if (_propertyKeysForDocument.Contains(Settings.Default.CustomDocumentDisplayIdentifier))
            {
                _setting.SortBy = Settings.Default.CustomDocumentDisplayIdentifier;
                if (Settings.Default.CustomDocumentDisplayIdentifier == "id")
                {
                    _setting.DisplayPattern = string.Format("{{{0}}}", Settings.Default.CustomDocumentDisplayIdentifier);
                }
                else
                {
                    _setting.DisplayPattern = string.Format("{{{0}}} [{{id}}]", Settings.Default.CustomDocumentDisplayIdentifier);
                }
            }
        }

        private void SetStatusForSettingFields(bool enabled)
        {
            tbSortField.Enabled = enabled;
            cbSortFields.Enabled = enabled;
            tbDisplayPattern.Enabled = enabled;

            var color = enabled ? SystemColors.ControlText : SystemColors.ControlDark;
            lblInfo1.ForeColor = color;
            lblInfo2.ForeColor = color;
            lblSortField.ForeColor = color;
            lblDisplayPattern.ForeColor = color;

            btnSettingToggle.Text = enabled ? "Remove setting" : "Create setting";

            if (enabled)
            {
                lbSettingInfo.Text = string.Format(
                    "A custom setting exists for document collection {0}. Click Remove setting if you want to remove it.", _documentCollectionId
                );
                gbCustom.Size = new Size {Height = 312, Width = gbCustom.Size.Width};
            }
            else
            {
                lbSettingInfo.Text = string.Format(
                    "There is currently no custom setting for document collection {0}. Click create setting to add one.", _documentCollectionId
                );
                gbCustom.Size = new Size { Height = 92, Width = gbCustom.Size.Width };
            }
        }

        private void SetSortSelectedIndexByValue(string value)
        {
            _setting.SortBy = value;
            try
            {
                cbSortFields.SelectedIndex = cbSortFields.FindStringExact(value);
            }
            catch (Exception){}
            ToggleSaveButtonAvailable();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ToggleSaveButtonAvailable();
            if (!btnSave.Enabled) return;

            if (_setting == null)
            {
                // Remove setting if there was one:
                _customDocumentListDisplayManager.RemoveCustomDocumentListDisplay(_hostName, _databaseId, _documentCollectionId);
            }
            else
            {
                // Add or update setting:
                _customDocumentListDisplayManager.SaveCustomDocumentListDisplay(_hostName, _setting);
            }

            Settings.Default.CustomDocumentDisplayIdentifier = tbDefaultGlobalFieldName.Text.Trim();
            Settings.Default.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cbSortFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbSortField.Text = cbSortFields.SelectedItem.ToString();
        }

        private void tbSortField_TextChanged(object sender, EventArgs e)
        {
            SetSortSelectedIndexByValue(tbSortField.Text);
        }

        private void tbDisplayPattern_TextChanged(object sender, EventArgs e)
        {
            _setting.DisplayPattern = tbDisplayPattern.Text.Trim();
            SetPatternExampleField();
            ToggleSaveButtonAvailable();
        }

        private void SetPatternExampleField()
        {
            string customDisplayPattern;
            string sortField;
            var docList = new List<dynamic> {_firstDocument};

            if (string.IsNullOrWhiteSpace(_setting.SortBy))
            {
                tbSortField.Text = "id";
            }

            var useCustom = _customDocumentListDisplayManager.GetCustomDocumentDisplayIdentifier(docList, _setting, tbDefaultGlobalFieldName.Text, out customDisplayPattern, out sortField, true);
            lbExample.Text = _customDocumentListDisplayManager.GetDisplayText(useCustom, _firstDocument, customDisplayPattern, useDirectAccess: true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            var dlgResult = MessageBox.Show("Are you sure that you want to ignore the changes?", "Ignore changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dlgResult == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnSettingToggle_Click(object sender, EventArgs e)
        {
            if (_setting == null)
            {
                CreateNewSetting();
                SetAllSettingFields();
            }
            else
            {
                var dlgResult = MessageBox.Show("Are you sure that you want to remove the setting?", "Remove setting?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult == DialogResult.Yes)
                {
                    _setting = null;
                }
            }
            SetStatusForSettingFields(_setting != null);
        }

        private void tbDefaultGlobalFieldName_TextChanged(object sender, EventArgs e)
        {
            ToggleSaveButtonAvailable();
        }

        private void ToggleSaveButtonAvailable()
        {
            var hintText = "";
            var isAvailable = true;
            if (string.IsNullOrWhiteSpace(tbDefaultGlobalFieldName.Text))
            {
                isAvailable = false;
                hintText = "Default field name must be filled in.";
            }

            if (tbDefaultGlobalFieldName.Text.Contains("\"") || tbDefaultGlobalFieldName.Text.Contains("{") || tbDefaultGlobalFieldName.Text.Contains("}") || tbDefaultGlobalFieldName.Text.Trim().Contains(" "))
            {
                isAvailable = false;
                hintText = "Default field name is invalid.";
            }


            if(_setting != null)
            {
                if (string.IsNullOrWhiteSpace(tbSortField.Text))
                {
                    isAvailable = false;
                    hintText = "Sort field must be filled in.";
                }

                if (string.IsNullOrWhiteSpace(tbDisplayPattern.Text))
                {
                    isAvailable = false;
                    hintText = "Display pattern must be filled in.";
                }
            }

            btnSave.Enabled = isAvailable;
            lblCannotSaveHint.Text = hintText;
        }

        private void cbReverseSort_CheckedChanged(object sender, EventArgs e)
        {
            _setting.ReverseSort = cbReverseSort.Checked;
        }
    }
}
