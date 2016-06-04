using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CimforceCaxTwPublic;

namespace ETableWork
{
    class StyleController
    {
//         StyleControlConfig config;
// 
//         // 建構函數，輸入配置檔路徑，讀取配置檔
//         public StyleController(string inputFullPath)
//         {
//             config = new StyleControlConfig();
//             ReadJsonData(inputFullPath, out config);
//         }


        // 定義顏色 (日後配置不同風格只要改這邊就好)
        private System.Drawing.Color backColor = System.Drawing.Color.WhiteSmoke;
        private System.Drawing.Color textColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
        private System.Drawing.Color transparentColor = System.Drawing.Color.Transparent;
        private System.Drawing.Color boxBackColor = System.Drawing.Color.White;
//         private System.Drawing.Color testColor = System.Drawing.Color.Red;

        // 控制項類型定義
//         Type FORMTYPE = new DevComponents.DotNetBar.Office2007Form.GetType();
        Type STYLE_MANAGER_TYPE = new DevComponents.DotNetBar.StyleManager().GetType();
        Type SUPERGRID_TYPE = new DevComponents.DotNetBar.SuperGrid.SuperGridControl().GetType();
        Type GROUP_BOX_TYPE = new System.Windows.Forms.GroupBox().GetType();
        Type GROUP_PANEL_TYPE = new DevComponents.DotNetBar.Controls.GroupPanel().GetType();
        Type LABEL_TYPE = new System.Windows.Forms.Label().GetType();
        Type DN_LABEL_TYPE = new DevComponents.DotNetBar.LabelX().GetType();
        Type DN_BUTTON_TYPE = new DevComponents.DotNetBar.ButtonX().GetType();
        Type CHECK_BOX_TYPE = new System.Windows.Forms.CheckBox().GetType();
        Type DN_CHECK_BOX_TYPE = new DevComponents.DotNetBar.Controls.CheckBoxX().GetType();
        Type TEXT_BOX_TYPE = new System.Windows.Forms.TextBox().GetType();
        Type DN_TEXT_BOX_TYPE = new DevComponents.DotNetBar.Controls.TextBoxX().GetType();
        Type RADIO_BUTTON_TYPE = new System.Windows.Forms.RadioButton().GetType();
        Type COMBO_BOX_TYPE = new System.Windows.Forms.ComboBox().GetType();
        Type DN_COMBO_BOX_TYPE = new DevComponents.DotNetBar.Controls.ComboBoxEx().GetType();
        Type TAB_TYPE = new System.Windows.Forms.TabControl().GetType();
        Type DN_TAB_TYPE = new DevComponents.DotNetBar.TabControl().GetType();
        Type DN_TAB_TYPE2 = new DevComponents.DotNetBar.TabControlPanel().GetType();

        public void SetAllStyle(DevComponents.DotNetBar.Office2007Form thisForm)
        {
            // 設定main form
            SetMainFormStyle(thisForm);
//             thisForm.get
            // 取得所有控制項
            List<System.Windows.Forms.Control> controlLst = new List<System.Windows.Forms.Control>();
            GetControls(thisForm, ref controlLst);
            foreach (System.Windows.Forms.Control oneControl in controlLst)
            {
                if (oneControl.GetType() == STYLE_MANAGER_TYPE)
                {
                    // 無法轉型!!  需在外面給
//                     SetStyleManager((DevComponents.DotNetBar.StyleManager)oneControl);
                }
                else if (oneControl.GetType() == SUPERGRID_TYPE)
                {
                    SetSupergridStyle((DevComponents.DotNetBar.SuperGrid.SuperGridControl)oneControl);
                }
                else if (oneControl.GetType() == GROUP_BOX_TYPE)
                {
                    SetGroupBoxStyle((System.Windows.Forms.GroupBox)oneControl);
                }
                else if (oneControl.GetType() == GROUP_PANEL_TYPE)
                {
                    SetGroupPanelStyle((DevComponents.DotNetBar.Controls.GroupPanel)oneControl);
                }
                else if (oneControl.GetType() == LABEL_TYPE)
                {
                    SetLabelStyle((System.Windows.Forms.Label)oneControl);
                }
                else if (oneControl.GetType() == DN_LABEL_TYPE)
                {
                    SetLabelStyle((DevComponents.DotNetBar.LabelX)oneControl);
                }
                else if (oneControl.GetType() == DN_BUTTON_TYPE)
                {
                    SetButtonStyle((DevComponents.DotNetBar.ButtonX)oneControl);
                }
                else if (oneControl.GetType() == CHECK_BOX_TYPE)
                {
                    SetCheckBoxStyle((System.Windows.Forms.CheckBox)oneControl);
                }
                else if (oneControl.GetType() == DN_CHECK_BOX_TYPE)
                {
                    SetCheckBoxStyle((DevComponents.DotNetBar.Controls.CheckBoxX)oneControl);
                }
                else if (oneControl.GetType() == TEXT_BOX_TYPE)
                {
                    SetTextBoxStyle((System.Windows.Forms.TextBox)oneControl);
                }
                else if (oneControl.GetType() == DN_TEXT_BOX_TYPE)
                {
                    SetTextBoxStyle((DevComponents.DotNetBar.Controls.TextBoxX)oneControl);
                }
                else if (oneControl.GetType() == RADIO_BUTTON_TYPE)
                {
                    SetRadioButtonStyle((System.Windows.Forms.RadioButton)oneControl);
                }
                else if (oneControl.GetType() == COMBO_BOX_TYPE)
                {
                    SetComboBoxStyle((System.Windows.Forms.ComboBox)oneControl);
                }
                else if (oneControl.GetType() == DN_COMBO_BOX_TYPE)
                {
                    SetComboBoxStyle((DevComponents.DotNetBar.Controls.ComboBoxEx)oneControl);
                }
//                 else if (oneControl.GetType() == TAB_TYPE)
//                 {
//                     SetTabStyle((System.Windows.Forms.TabControl)oneControl);
//                 }
                else if (oneControl.GetType() == DN_TAB_TYPE)
                {
                    SetTabStyle((DevComponents.DotNetBar.TabControl)oneControl);
                }
                else if (oneControl.GetType() == DN_TAB_TYPE2)
                {
                    SetTabStyle((DevComponents.DotNetBar.TabControlPanel)oneControl);
                }
            }
        }

        // 抓所有控制項(遞迴)
        private void GetControls(System.Windows.Forms.Control inputControl, ref List<System.Windows.Forms.Control> controlLst)
        {
            controlLst.Add(inputControl);
            if (inputControl.HasChildren)
            {
                for (int i = 0; i < inputControl.Controls.Count; i++)
                {
                    GetControls(inputControl.Controls[i], ref controlLst);
                }
            }
            return;
        }

        // MainForm
        public void SetMainFormStyle(DevComponents.DotNetBar.Office2007Form thisForm)
        {
            //             thisForm.EnableGlass = false;
            thisForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            thisForm.BackColor = backColor;
            thisForm.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            thisForm.TopLeftCornerSize = 0;
            thisForm.TopRightCornerSize = 0;
        }

        // StyleManager
        public void SetStyleManager(DevComponents.DotNetBar.StyleManager thisStyleManager)
        {
            thisStyleManager.ManagerColorTint = System.Drawing.SystemColors.HotTrack;
            thisStyleManager.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
            thisStyleManager.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(backColor, textColor);
        }

        // Supergrid
        public void SetSupergridStyle(DevComponents.DotNetBar.SuperGrid.SuperGridControl thisSupergrid)
        {
            thisSupergrid.PrimaryGrid.ShowRowHeaders = false;
            thisSupergrid.BackColor = boxBackColor;
            thisSupergrid.ForeColor = textColor;
            for (int i = 0; i < thisSupergrid.PrimaryGrid.Columns.Count; i++)
            {
                thisSupergrid.PrimaryGrid.Columns[i].HeaderStyles.Default.Background.Color1 = boxBackColor;
                thisSupergrid.PrimaryGrid.Columns[i].HeaderStyles.Default.Background.Color2 = boxBackColor;
                thisSupergrid.PrimaryGrid.Columns[i].HeaderStyles.Default.TextColor = textColor;
            }
        }

        // GroupBox
        public void SetGroupBoxStyle(System.Windows.Forms.GroupBox thisGroupBox)
        {
            thisGroupBox.BackColor = transparentColor;
            thisGroupBox.ForeColor = textColor;
            thisGroupBox.Font = new System.Drawing.Font("微軟正黑體", thisGroupBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // GroupPanel
        public void SetGroupPanelStyle(DevComponents.DotNetBar.Controls.GroupPanel thisGroupPanel)
        {
            thisGroupPanel.BackColor = transparentColor;
            thisGroupPanel.CanvasColor = transparentColor;
            thisGroupPanel.Style.BackColor = transparentColor;
            thisGroupPanel.Style.BackColor2 = transparentColor;
            thisGroupPanel.Style.BorderColor = textColor;
            thisGroupPanel.Style.TextColor = textColor;
            thisGroupPanel.Font = new System.Drawing.Font("微軟正黑體", thisGroupPanel.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // Label
        public void SetLabelStyle(System.Windows.Forms.Label thisLabel)
        {
            thisLabel.BackColor = transparentColor;
            thisLabel.ForeColor = textColor;
            thisLabel.Font = new System.Drawing.Font("微軟正黑體", thisLabel.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // DotNetLabel
        public void SetLabelStyle(DevComponents.DotNetBar.LabelX thisLabel)
        {
            thisLabel.BackColor = transparentColor;
            thisLabel.ForeColor = textColor;
            thisLabel.Font = new System.Drawing.Font("微軟正黑體", thisLabel.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // Button
        // 處理不了QQ

        // DotNetButton
        public void SetButtonStyle(DevComponents.DotNetBar.ButtonX thisButton)
        {
            thisButton.TextColor = textColor;
            thisButton.ColorTable = DevComponents.DotNetBar.eButtonColor.BlueWithBackground;
            thisButton.Font = new System.Drawing.Font("微軟正黑體", thisButton.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }
        // CheckBox
        public void SetCheckBoxStyle(System.Windows.Forms.CheckBox thisCheckBox)
        {
            thisCheckBox.BackColor = backColor;
            thisCheckBox.ForeColor = textColor;
            thisCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            thisCheckBox.Font = new System.Drawing.Font("微軟正黑體", thisCheckBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }
        // DotNetCheckBox
        public void SetCheckBoxStyle(DevComponents.DotNetBar.Controls.CheckBoxX thisCheckBox)
        {
            thisCheckBox.BackColor = backColor;
            thisCheckBox.ForeColor = textColor;
            thisCheckBox.Font = new System.Drawing.Font("微軟正黑體", thisCheckBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }
        // TextBox
        public void SetTextBoxStyle(System.Windows.Forms.TextBox thisTextBox)
        {
            thisTextBox.BackColor = boxBackColor;
            thisTextBox.ForeColor = textColor;
            thisTextBox.Font = new System.Drawing.Font("微軟正黑體", thisTextBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // DotNetTextBox
        public void SetTextBoxStyle(DevComponents.DotNetBar.Controls.TextBoxX thisTextBox)
        {
            thisTextBox.BackColor = boxBackColor;
            thisTextBox.ForeColor = textColor;
            thisTextBox.WatermarkColor = textColor;
            thisTextBox.Font = new System.Drawing.Font("微軟正黑體", thisTextBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // RadioButton
        public void SetRadioButtonStyle(System.Windows.Forms.RadioButton thisRadioButton)
        {
            thisRadioButton.BackColor = backColor;
            thisRadioButton.ForeColor = textColor;
            thisRadioButton.Font = new System.Drawing.Font("微軟正黑體", thisRadioButton.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // ComboBox
        public void SetComboBoxStyle(System.Windows.Forms.ComboBox thisComboBox)
        {
            thisComboBox.BackColor = boxBackColor;
            thisComboBox.ForeColor = textColor;
            thisComboBox.Font = new System.Drawing.Font("微軟正黑體", thisComboBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // ComboBoxEX
        public void SetComboBoxStyle(DevComponents.DotNetBar.Controls.ComboBoxEx thisComboBox)
        {
            thisComboBox.BackColor = boxBackColor;
            thisComboBox.ForeColor = textColor;
            thisComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            thisComboBox.Font = new System.Drawing.Font("微軟正黑體", thisComboBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // Tab 無解QQ
//         public void SetTabStyle(System.Windows.Forms.TabControl thisTab)
//         {
//             for (int i = 0; i < thisTab.TabPages.Count; i++)
//             {
//                 thisTab.TabPages[i].BackColor = transparentColor;
//                 thisTab.TabPages[i].ForeColor = textColor;
//                 thisTab.Font = new System.Drawing.Font("微軟正黑體", thisTab.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
//             }
//         }

        public void SetTabStyle(DevComponents.DotNetBar.TabControl thisTab)
        {
            thisTab.BackColor = transparentColor;
            thisTab.ForeColor = textColor;

            thisTab.ColorScheme.TabBackground = backColor;
            thisTab.ColorScheme.TabBackground2 = backColor;
            thisTab.ColorScheme.TabBorder = textColor;

            thisTab.ColorScheme.TabItemText = textColor;
            thisTab.ColorScheme.TabItemBackground = backColor;
            thisTab.ColorScheme.TabItemBackground2 = backColor;
            thisTab.ColorScheme.TabItemBorder = textColor;
            thisTab.ColorScheme.TabItemBorderDark = textColor;
            thisTab.ColorScheme.TabItemBorderLight = textColor;

            thisTab.ColorScheme.TabItemHotText = textColor;
            thisTab.ColorScheme.TabItemHotBackground = backColor;
            thisTab.ColorScheme.TabItemHotBackground2 = backColor;
            thisTab.ColorScheme.TabItemHotBorder = textColor;
            thisTab.ColorScheme.TabItemHotBorderDark = textColor;
            thisTab.ColorScheme.TabItemHotBorderLight = textColor;

            thisTab.ColorScheme.TabItemSelectedText = textColor;
            thisTab.ColorScheme.TabItemSelectedBackground = backColor;
            thisTab.ColorScheme.TabItemSelectedBackground2 = backColor;
            thisTab.ColorScheme.TabItemSelectedBorder = textColor;
            thisTab.ColorScheme.TabItemSelectedBorderDark = textColor;
            thisTab.ColorScheme.TabItemSelectedBorderLight = textColor;

            thisTab.Style = DevComponents.DotNetBar.eTabStripStyle.VS2005;
            thisTab.Font = new System.Drawing.Font("微軟正黑體", thisTab.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            for (int i = 0; i < thisTab.Tabs.Count; i++)
            {
                SetTabItemStyle(thisTab.Tabs[i]);
            }
        }
        public void SetTabItemStyle(DevComponents.DotNetBar.TabItem thisTab)
        {
            thisTab.BackColor = backColor;
            thisTab.BackColor2 = backColor;
            thisTab.BorderColor = textColor;
            thisTab.DarkBorderColor = textColor;
            thisTab.LightBorderColor = textColor;
            thisTab.TextColor = textColor;
        }
        public void SetTabStyle(DevComponents.DotNetBar.TabControlPanel thisTab)
        {
            thisTab.BackColor = backColor;
            thisTab.CanvasColor = backColor;
            thisTab.Style.BackColor1.Color = backColor;
            thisTab.Style.BackColor2.Color = backColor;
            thisTab.Style.BorderColor.Color = textColor;
            thisTab.Style.ForeColor.Color = textColor;

            thisTab.StyleMouseDown.BackColor1.Color = backColor;
            thisTab.StyleMouseDown.BackColor2.Color = backColor;
            thisTab.StyleMouseDown.BorderColor.Color = textColor;
            thisTab.StyleMouseDown.ForeColor.Color = textColor;

            thisTab.StyleMouseOver.BackColor1.Color = backColor;
            thisTab.StyleMouseOver.BackColor2.Color = backColor;
            thisTab.StyleMouseOver.BorderColor.Color = textColor;
            thisTab.StyleMouseOver.ForeColor.Color = textColor;
        }
    }

}
