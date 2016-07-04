using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaxUGforEspritt
{
    class StyleController
    {
        // 定義顏色 (日後配置不同風格只要改這邊就好)
        public System.Drawing.Color backColor = System.Drawing.Color.WhiteSmoke;
        public System.Drawing.Color textColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
        //System.Drawing.Color.Black;//
        public System.Drawing.Color transparentColor = System.Drawing.Color.Transparent;
        public System.Drawing.Color boxBackColor = System.Drawing.Color.White;
        private System.Drawing.Color testColor = System.Drawing.Color.Red;

        // 其他寫死的風格
        public System.Drawing.Color managerColorTint = System.Drawing.SystemColors.HotTrack;
        public DevComponents.DotNetBar.eStyle managerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue;
        public DevComponents.DotNetBar.eButtonColor buttonColor = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
        public DevComponents.DotNetBar.eTabStripStyle tabStyle = DevComponents.DotNetBar.eTabStripStyle.VS2005;
        public DevComponents.DotNetBar.eSuperTabStyle superTabStyle = DevComponents.DotNetBar.eSuperTabStyle.Office2007;
        public string controlFont = "微軟正黑體";

        // 控制項類型定義
//         Type FORMTYPE = new DevComponents.DotNetBar.Office2007Form.GetType();
        private Type STYLE_MANAGER_TYPE = new DevComponents.DotNetBar.StyleManager().GetType();
        private Type SUPERGRID_TYPE = new DevComponents.DotNetBar.SuperGrid.SuperGridControl().GetType();
        private Type GROUP_BOX_TYPE = new System.Windows.Forms.GroupBox().GetType();
        private Type GROUP_PANEL_TYPE = new DevComponents.DotNetBar.Controls.GroupPanel().GetType();
        private Type LABEL_TYPE = new System.Windows.Forms.Label().GetType();
        private Type DN_LABEL_TYPE = new DevComponents.DotNetBar.LabelX().GetType();
        private Type DN_BUTTON_TYPE = new DevComponents.DotNetBar.ButtonX().GetType();
        private Type CHECK_BOX_TYPE = new System.Windows.Forms.CheckBox().GetType();
        private Type DN_CHECK_BOX_TYPE = new DevComponents.DotNetBar.Controls.CheckBoxX().GetType();
        private Type TEXT_BOX_TYPE = new System.Windows.Forms.TextBox().GetType();
        private Type DN_TEXT_BOX_TYPE = new DevComponents.DotNetBar.Controls.TextBoxX().GetType();
        private Type RADIO_BUTTON_TYPE = new System.Windows.Forms.RadioButton().GetType();
        private Type COMBO_BOX_TYPE = new System.Windows.Forms.ComboBox().GetType();
        private Type DN_COMBO_BOX_TYPE = new DevComponents.DotNetBar.Controls.ComboBoxEx().GetType();
        private Type TAB_TYPE = new System.Windows.Forms.TabControl().GetType();
        private Type DN_TAB_TYPE = new DevComponents.DotNetBar.TabControl().GetType();
        private Type DN_TAB_PANEL_TYPE = new DevComponents.DotNetBar.TabControlPanel().GetType();
        private Type DN_SUPER_TAB_TYPE = new DevComponents.DotNetBar.SuperTabControl().GetType();
        private Type DN_SUPER_TAB_ITEM_TYPE = new DevComponents.DotNetBar.SuperTabItem().GetType();
        private Type DN_SUPER_TAB_PANEL_TYPE = new DevComponents.DotNetBar.SuperTabControlPanel().GetType();
        private Type DN_METRO_TILE_PANEL_TYPE = new DevComponents.DotNetBar.Metro.MetroTilePanel().GetType();
        

        public void SetAllStyle(DevComponents.DotNetBar.Office2007Form thisForm)
        {
            // 設定main form
            SetMainFormStyle(thisForm);

            // 取得所有控制項
            List<System.Windows.Forms.Control> controlLst = new List<System.Windows.Forms.Control>();
            GetControls(thisForm, ref controlLst);

            // 判斷類型丟到對應的function
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
                else if (oneControl.GetType() == DN_TAB_PANEL_TYPE)
                {
                    SetTabPanelStyle((DevComponents.DotNetBar.TabControlPanel)oneControl);
                }
                else if (oneControl.GetType() == DN_SUPER_TAB_TYPE)
                {
                    SetSuperTabStyle((DevComponents.DotNetBar.SuperTabControl)oneControl);
                }
                else if (oneControl.GetType() == DN_SUPER_TAB_PANEL_TYPE)
                {
                    SetSuperTabPanelStyle((DevComponents.DotNetBar.SuperTabControlPanel)oneControl);
                }
                else if (oneControl.GetType() == DN_METRO_TILE_PANEL_TYPE)
                {
                    SetMetroTilePanelStyle((DevComponents.DotNetBar.Metro.MetroTilePanel)oneControl);
                }
            }
        }

        // 抓所有控制項(遞迴)
        public void GetControls(System.Windows.Forms.Control inputControl, ref List<System.Windows.Forms.Control> controlLst)
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
            thisStyleManager.ManagerColorTint = managerColorTint;
            thisStyleManager.ManagerStyle = managerStyle;
            thisStyleManager.MetroColorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(backColor, textColor);
        }

        // Supergrid
        public void SetSupergridStyle(DevComponents.DotNetBar.SuperGrid.SuperGridControl thisSupergrid)
        {
            thisSupergrid.PrimaryGrid.ShowRowHeaders = false;
            thisSupergrid.PrimaryGrid.DefaultVisualStyles.GridPanelStyle.Background.Color1 = boxBackColor;
            thisSupergrid.PrimaryGrid.DefaultVisualStyles.GridPanelStyle.Background.Color2 = boxBackColor;
            thisSupergrid.PrimaryGrid.DefaultVisualStyles.ColumnHeaderRowStyles.Default.WhiteSpaceBackground.Color1 = boxBackColor;
            thisSupergrid.PrimaryGrid.DefaultVisualStyles.ColumnHeaderRowStyles.Default.WhiteSpaceBackground.Color2 = boxBackColor;
            thisSupergrid.BackColor = boxBackColor;
            thisSupergrid.ForeColor = textColor;
            for (int i = 0; i < thisSupergrid.PrimaryGrid.Columns.Count; i++)
            {
                thisSupergrid.PrimaryGrid.Columns[i].HeaderStyles.Default.Background.Color1 = boxBackColor;
                thisSupergrid.PrimaryGrid.Columns[i].HeaderStyles.Default.Background.Color2 = boxBackColor;
                thisSupergrid.PrimaryGrid.Columns[i].HeaderStyles.Default.TextColor = textColor;
                thisSupergrid.PrimaryGrid.Columns[i].CellStyles.Default.Background.Color1 = boxBackColor;
                thisSupergrid.PrimaryGrid.Columns[i].CellStyles.Default.Background.Color2 = boxBackColor;
                thisSupergrid.PrimaryGrid.Columns[i].CellStyles.Default.TextColor = textColor;
            }
        }

        // GroupBox
        public void SetGroupBoxStyle(System.Windows.Forms.GroupBox thisGroupBox)
        {
            thisGroupBox.BackColor = transparentColor;
            thisGroupBox.ForeColor = textColor;
            thisGroupBox.Font = new System.Drawing.Font(controlFont, thisGroupBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
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
            thisGroupPanel.DrawTitleBox = false;
//             thisGroupPanel.Style. = DevComponents.DotNetBar.eColorSchemePart.None;
            thisGroupPanel.Font = new System.Drawing.Font(controlFont, thisGroupPanel.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // Label
        public void SetLabelStyle(System.Windows.Forms.Label thisLabel)
        {
            thisLabel.BackColor = transparentColor;
            thisLabel.ForeColor = textColor;
            thisLabel.Font = new System.Drawing.Font(controlFont, thisLabel.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // DotNetLabel
        public void SetLabelStyle(DevComponents.DotNetBar.LabelX thisLabel)
        {
            thisLabel.BackColor = transparentColor;
            thisLabel.ForeColor = textColor;
            thisLabel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            thisLabel.Font = new System.Drawing.Font(controlFont, thisLabel.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // Button
        // 處理不了QQ

        // DotNetButton
        public void SetButtonStyle(DevComponents.DotNetBar.ButtonX thisButton)
        {
            thisButton.TextColor = textColor;
            thisButton.ColorTable = buttonColor;
            thisButton.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            thisButton.Font = new System.Drawing.Font(controlFont, thisButton.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // CheckBox
        public void SetCheckBoxStyle(System.Windows.Forms.CheckBox thisCheckBox)
        {
            thisCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            thisCheckBox.BackColor = backColor;
            thisCheckBox.ForeColor = textColor;
            thisCheckBox.Font = new System.Drawing.Font(controlFont, thisCheckBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // DotNetCheckBox
        public void SetCheckBoxStyle(DevComponents.DotNetBar.Controls.CheckBoxX thisCheckBox)
        {
            thisCheckBox.BackColor = backColor;
            thisCheckBox.ForeColor = textColor;
            thisCheckBox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            thisCheckBox.Font = new System.Drawing.Font(controlFont, thisCheckBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // TextBox
        public void SetTextBoxStyle(System.Windows.Forms.TextBox thisTextBox)
        {
            thisTextBox.BackColor = boxBackColor;
            thisTextBox.ForeColor = textColor;
            thisTextBox.Font = new System.Drawing.Font(controlFont, thisTextBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // DotNetTextBox
        public void SetTextBoxStyle(DevComponents.DotNetBar.Controls.TextBoxX thisTextBox)
        {
            thisTextBox.BackColor = boxBackColor;
            thisTextBox.ForeColor = textColor;
            thisTextBox.FocusHighlightColor = textColor;
            thisTextBox.WatermarkColor = textColor;
            thisTextBox.Border.BackColor = boxBackColor;
            thisTextBox.Border.BackColor2 = boxBackColor;
            thisTextBox.Border.BorderColor = textColor;
            thisTextBox.Border.BorderColor2 = textColor;
            thisTextBox.Border.TextColor = textColor;
            thisTextBox.Font = new System.Drawing.Font(controlFont, thisTextBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // RadioButton
        public void SetRadioButtonStyle(System.Windows.Forms.RadioButton thisRadioButton)
        {
            thisRadioButton.BackColor = backColor;
            thisRadioButton.ForeColor = textColor;
            thisRadioButton.Font = new System.Drawing.Font(controlFont, thisRadioButton.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // ComboBox
        public void SetComboBoxStyle(System.Windows.Forms.ComboBox thisComboBox)
        {
            thisComboBox.BackColor = boxBackColor;
            thisComboBox.ForeColor = textColor;
            thisComboBox.Font = new System.Drawing.Font(controlFont, thisComboBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
        }

        // ComboBoxEX
        public void SetComboBoxStyle(DevComponents.DotNetBar.Controls.ComboBoxEx thisComboBox)
        {
            thisComboBox.BackColor = boxBackColor;
            thisComboBox.ForeColor = textColor;
            thisComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            thisComboBox.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            thisComboBox.Font = new System.Drawing.Font(controlFont, thisComboBox.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
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

        // DotNetTab
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

            thisTab.Style = tabStyle;
            thisTab.Font = new System.Drawing.Font(controlFont, thisTab.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            thisTab.SelectedTabFont = new System.Drawing.Font(controlFont, thisTab.Font.Size, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            
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
        public void SetTabPanelStyle(DevComponents.DotNetBar.TabControlPanel thisTab)
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

        // superTab
        public void SetSuperTabStyle(DevComponents.DotNetBar.SuperTabControl thisTab)
        {
            thisTab.BackColor = backColor;
            thisTab.ForeColor = textColor;
            thisTab.TabStyle = superTabStyle;
            thisTab.TabStripColor.InnerBorder = backColor;
            thisTab.TabStripColor.OuterBorder = textColor;
            thisTab.TabStripColor.Background.Colors = new System.Drawing.Color[] { backColor };

            thisTab.Font = new System.Drawing.Font(controlFont, thisTab.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            thisTab.TabFont = new System.Drawing.Font(controlFont, thisTab.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            thisTab.SelectedTabFont = new System.Drawing.Font(controlFont, thisTab.Font.Size, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));

            for (int j = 0; j < thisTab.Tabs.Count; j++)
            {
                if (thisTab.Tabs[j].GetType() == DN_SUPER_TAB_ITEM_TYPE)
                {
                    SetSuperTabItemStyle((DevComponents.DotNetBar.SuperTabItem)thisTab.Tabs[j]);
                }
            }
        }
        public void SetSuperTabItemStyle(DevComponents.DotNetBar.SuperTabItem thisTab)
        {
//             thisTab.TabColor.Bottom
//             thisTab.TabColor.Bottom.Normal.Background.Colors = new System.Drawing.Color[] { testColor };
//             thisTab.TabColor.Bottom.Selected.Background.Colors = new System.Drawing.Color[] { testColor };
//             thisTab.TabColor.Bottom.MouseOver.Background.Colors = new System.Drawing.Color[] { testColor };
//             thisTab.TabColor.Bottom.SelectedMouseOver.Background.Colors = new System.Drawing.Color[] { testColor };
//             thisTab.TabColor.Default.MouseOver
            thisTab.TabColor.Default.Normal.Text = textColor;
            thisTab.TabColor.Default.Normal.InnerBorder = backColor;
            thisTab.TabColor.Default.Normal.OuterBorder = textColor;
            thisTab.TabColor.Default.Normal.Background.Colors = new System.Drawing.Color[] { backColor };
            thisTab.TabColor.Default.Selected.Text = textColor;
            thisTab.TabColor.Default.Selected.InnerBorder = backColor;
            thisTab.TabColor.Default.Selected.OuterBorder = textColor;
            thisTab.TabColor.Default.Selected.Background.Colors = new System.Drawing.Color[] { backColor };
            thisTab.TabColor.Default.MouseOver.Text = textColor;
            thisTab.TabColor.Default.MouseOver.InnerBorder = backColor;
            thisTab.TabColor.Default.MouseOver.OuterBorder = textColor;
            thisTab.TabColor.Default.MouseOver.Background.Colors = new System.Drawing.Color[] { backColor };
            thisTab.TabColor.Default.SelectedMouseOver.Text = textColor;
            thisTab.TabColor.Default.SelectedMouseOver.InnerBorder = backColor;
            thisTab.TabColor.Default.SelectedMouseOver.OuterBorder = textColor;
            thisTab.TabColor.Default.SelectedMouseOver.Background.Colors = new System.Drawing.Color[] { backColor };

//             thisTab.TabColor.Default.MouseOver.Text
        }
        public void SetSuperTabPanelStyle(DevComponents.DotNetBar.SuperTabControlPanel thisTab)
        {
            thisTab.CanvasColor = backColor;
            thisTab.ColorScheme.BarStripeColor = backColor;
            thisTab.PanelColor.Default.InnerBorder = backColor;
            thisTab.PanelColor.Default.OuterBorder = textColor;
            thisTab.PanelColor.Default.Background.Colors = new System.Drawing.Color[] { backColor };
            thisTab.PanelColor.Bottom.InnerBorder = backColor;
            thisTab.PanelColor.Bottom.OuterBorder = textColor;
            thisTab.PanelColor.Bottom.Background.Colors = new System.Drawing.Color[] { backColor };
            thisTab.PanelColor.Left.InnerBorder = backColor;
            thisTab.PanelColor.Left.OuterBorder = textColor;
            thisTab.PanelColor.Left.Background.Colors = new System.Drawing.Color[] { backColor };
            thisTab.PanelColor.Right.InnerBorder = backColor;
            thisTab.PanelColor.Right.OuterBorder = textColor;
            thisTab.PanelColor.Right.Background.Colors = new System.Drawing.Color[] { backColor };

        }

        // metroTilePanel
        public void SetMetroTilePanelStyle(DevComponents.DotNetBar.Metro.MetroTilePanel thisPanel)
        {
            thisPanel.ForeColor = textColor;
            thisPanel.BackColor = backColor;
            thisPanel.BackgroundStyle.BackColor = backColor;
            thisPanel.BackgroundStyle.BackColor2 = backColor;
            thisPanel.BackgroundStyle.BorderColor = textColor;
            thisPanel.BackgroundStyle.BorderColor2 = textColor;
            thisPanel.BackgroundStyle.BorderColorLight = textColor;
            thisPanel.BackgroundStyle.BorderColorLight2 = textColor;
            thisPanel.BackgroundStyle.TextColor = textColor;
        }
    }

}
