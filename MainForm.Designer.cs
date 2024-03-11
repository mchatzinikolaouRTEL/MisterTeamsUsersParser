
using MisterTeamsUsersParserParser.Helpers;

namespace MisterTeamsUsersParserParser
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.serviceInfoPanel = new System.Windows.Forms.Panel();
            this.closeWindowCheckBox = new System.Windows.Forms.CheckBox();
            this.uninstallServiceButton = new System.Windows.Forms.Button();
            this.commandLineoptionsStaticLabel = new System.Windows.Forms.Label();
            this.AdministratorCheckBox = new System.Windows.Forms.CheckBox();
            this.installServiceButton = new System.Windows.Forms.Button();
            this.serviceStatusLabel = new System.Windows.Forms.Label();
            this.serviceStatusContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyInstallCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopstartAutoCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serviceStaticLabel = new System.Windows.Forms.Label();
            this.LeftPanel = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ParseTeamsUsersButton = new System.Windows.Forms.Button();
            this.LogsListBox = new System.Windows.Forms.ListBox();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.serviceInfoPanel.SuspendLayout();
            this.serviceStatusContextMenuStrip.SuspendLayout();
            this.LeftPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // serviceInfoPanel
            // 
            this.serviceInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serviceInfoPanel.Controls.Add(this.closeWindowCheckBox);
            this.serviceInfoPanel.Controls.Add(this.uninstallServiceButton);
            this.serviceInfoPanel.Controls.Add(this.commandLineoptionsStaticLabel);
            this.serviceInfoPanel.Controls.Add(this.AdministratorCheckBox);
            this.serviceInfoPanel.Controls.Add(this.installServiceButton);
            this.serviceInfoPanel.Controls.Add(this.serviceStatusLabel);
            this.serviceInfoPanel.Controls.Add(this.serviceStaticLabel);
            this.serviceInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.serviceInfoPanel.Location = new System.Drawing.Point(0, 0);
            this.serviceInfoPanel.Name = "serviceInfoPanel";
            this.serviceInfoPanel.Size = new System.Drawing.Size(281, 109);
            this.serviceInfoPanel.TabIndex = 0;
            // 
            // closeWindowCheckBox
            // 
            this.closeWindowCheckBox.AutoSize = true;
            this.closeWindowCheckBox.Checked = true;
            this.closeWindowCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.closeWindowCheckBox.Location = new System.Drawing.Point(12, 77);
            this.closeWindowCheckBox.Name = "closeWindowCheckBox";
            this.closeWindowCheckBox.Size = new System.Drawing.Size(102, 19);
            this.closeWindowCheckBox.TabIndex = 7;
            this.closeWindowCheckBox.Text = "Close Window";
            this.closeWindowCheckBox.UseVisualStyleBackColor = true;
            // 
            // uninstallServiceButton
            // 
            this.uninstallServiceButton.Enabled = false;
            this.uninstallServiceButton.Location = new System.Drawing.Point(138, 77);
            this.uninstallServiceButton.Name = "uninstallServiceButton";
            this.uninstallServiceButton.Size = new System.Drawing.Size(141, 23);
            this.uninstallServiceButton.TabIndex = 5;
            this.uninstallServiceButton.Text = "Uninstall service";
            this.uninstallServiceButton.UseVisualStyleBackColor = true;
            this.uninstallServiceButton.Click += new System.EventHandler(this.UninstallServiceButton_Click);
            // 
            // commandLineoptionsStaticLabel
            // 
            this.commandLineoptionsStaticLabel.AutoSize = true;
            this.commandLineoptionsStaticLabel.Location = new System.Drawing.Point(3, 34);
            this.commandLineoptionsStaticLabel.Name = "commandLineoptionsStaticLabel";
            this.commandLineoptionsStaticLabel.Size = new System.Drawing.Size(129, 15);
            this.commandLineoptionsStaticLabel.TabIndex = 6;
            this.commandLineoptionsStaticLabel.Text = "Command line options";
            // 
            // AdministratorCheckBox
            // 
            this.AdministratorCheckBox.AutoSize = true;
            this.AdministratorCheckBox.Checked = true;
            this.AdministratorCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AdministratorCheckBox.Location = new System.Drawing.Point(12, 52);
            this.AdministratorCheckBox.Name = "AdministratorCheckBox";
            this.AdministratorCheckBox.Size = new System.Drawing.Size(99, 19);
            this.AdministratorCheckBox.TabIndex = 1;
            this.AdministratorCheckBox.Text = "Administrator";
            this.AdministratorCheckBox.UseVisualStyleBackColor = true;
            // 
            // installServiceButton
            // 
            this.installServiceButton.Enabled = false;
            this.installServiceButton.Location = new System.Drawing.Point(138, 48);
            this.installServiceButton.Name = "installServiceButton";
            this.installServiceButton.Size = new System.Drawing.Size(141, 23);
            this.installServiceButton.TabIndex = 4;
            this.installServiceButton.Text = "Install as Servrice";
            this.installServiceButton.UseVisualStyleBackColor = true;
            this.installServiceButton.Click += new System.EventHandler(this.InstallServiceButton_Click);
            // 
            // serviceStatusLabel
            // 
            this.serviceStatusLabel.AutoSize = true;
            this.serviceStatusLabel.ContextMenuStrip = this.serviceStatusContextMenuStrip;
            this.serviceStatusLabel.ForeColor = System.Drawing.Color.Black;
            this.serviceStatusLabel.Location = new System.Drawing.Point(138, 8);
            this.serviceStatusLabel.Name = "serviceStatusLabel";
            this.serviceStatusLabel.Size = new System.Drawing.Size(74, 15);
            this.serviceStatusLabel.TabIndex = 3;
            this.serviceStatusLabel.Text = "Not checked";
            // 
            // serviceStatusContextMenuStrip
            // 
            this.serviceStatusContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyInstallCommandToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.checkNowToolStripMenuItem,
            this.stopstartAutoCheckToolStripMenuItem});
            this.serviceStatusContextMenuStrip.Name = "serviceStatusContextMenuStrip";
            this.serviceStatusContextMenuStrip.Size = new System.Drawing.Size(196, 92);
            this.serviceStatusContextMenuStrip.Text = "Service cmd commands";
            // 
            // copyInstallCommandToolStripMenuItem
            // 
            this.copyInstallCommandToolStripMenuItem.Name = "copyInstallCommandToolStripMenuItem";
            this.copyInstallCommandToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.copyInstallCommandToolStripMenuItem.Text = "Copy create command";
            this.copyInstallCommandToolStripMenuItem.Click += new System.EventHandler(this.CopyInstallCommandToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.copyToolStripMenuItem.Text = "Copy delete command";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // checkNowToolStripMenuItem
            // 
            this.checkNowToolStripMenuItem.Name = "checkNowToolStripMenuItem";
            this.checkNowToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.checkNowToolStripMenuItem.Text = "Check now";
            this.checkNowToolStripMenuItem.Click += new System.EventHandler(this.CheckNowToolStripMenuItem_Click);
            // 
            // stopstartAutoCheckToolStripMenuItem
            // 
            this.stopstartAutoCheckToolStripMenuItem.Name = "stopstartAutoCheckToolStripMenuItem";
            this.stopstartAutoCheckToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.stopstartAutoCheckToolStripMenuItem.Text = "Stop auto check";
            this.stopstartAutoCheckToolStripMenuItem.Click += new System.EventHandler(this.StopstartAutoCheckToolStripMenuItem_Click);
            // 
            // serviceStaticLabel
            // 
            this.serviceStaticLabel.AutoSize = true;
            this.serviceStaticLabel.Location = new System.Drawing.Point(50, 8);
            this.serviceStaticLabel.Name = "serviceStaticLabel";
            this.serviceStaticLabel.Size = new System.Drawing.Size(82, 15);
            this.serviceStaticLabel.TabIndex = 0;
            this.serviceStaticLabel.Text = "Service Status:";
            // 
            // LeftPanel
            // 
            this.LeftPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LeftPanel.Controls.Add(this.label12);
            this.LeftPanel.Controls.Add(this.label11);
            this.LeftPanel.Controls.Add(this.label10);
            this.LeftPanel.Controls.Add(this.label9);
            this.LeftPanel.Controls.Add(this.label8);
            this.LeftPanel.Controls.Add(this.label7);
            this.LeftPanel.Controls.Add(this.label6);
            this.LeftPanel.Controls.Add(this.label5);
            this.LeftPanel.Controls.Add(this.label4);
            this.LeftPanel.Controls.Add(this.label3);
            this.LeftPanel.Controls.Add(this.label2);
            this.LeftPanel.Controls.Add(this.label1);
            this.LeftPanel.Controls.Add(this.serviceInfoPanel);
            this.LeftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftPanel.Name = "LeftPanel";
            this.LeftPanel.Size = new System.Drawing.Size(283, 361);
            this.LeftPanel.TabIndex = 2;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(4, 321);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(220, 19);
            this.label12.TabIndex = 23;
            this.label12.Text = "label12";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label12.Visible = false;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(4, 302);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(220, 19);
            this.label11.TabIndex = 21;
            this.label11.Text = "label11";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label11.Visible = false;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(4, 283);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(220, 19);
            this.label10.TabIndex = 19;
            this.label10.Text = "label10";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label10.Visible = false;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(4, 264);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(220, 19);
            this.label9.TabIndex = 17;
            this.label9.Text = "label9";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label9.Visible = false;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(4, 245);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(220, 19);
            this.label8.TabIndex = 15;
            this.label8.Text = "label8";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label8.Visible = false;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(4, 226);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(220, 19);
            this.label7.TabIndex = 13;
            this.label7.Text = "label7";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label7.Visible = false;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(4, 207);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(220, 19);
            this.label6.TabIndex = 11;
            this.label6.Text = "label6";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label6.Visible = false;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(4, 188);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(220, 19);
            this.label5.TabIndex = 9;
            this.label5.Text = "label5";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.Visible = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(4, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(220, 19);
            this.label4.TabIndex = 7;
            this.label4.Text = "label4";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label4.Visible = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(4, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(220, 19);
            this.label3.TabIndex = 5;
            this.label3.Text = "label3";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Visible = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(220, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label2.Visible = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label1.Visible = false;
            // 
            // Parse Team users button
            // 
            this.ParseTeamsUsersButton.AutoSize = true;
            this.ParseTeamsUsersButton.Location = new System.Drawing.Point(6, 3);
            this.ParseTeamsUsersButton.Name = "ParseTeamsUsersButton";
            this.ParseTeamsUsersButton.Size = new System.Drawing.Size(124, 25);
            this.ParseTeamsUsersButton.TabIndex = 4;
            this.ParseTeamsUsersButton.Text = "Parse Action 01";
            this.ParseTeamsUsersButton.UseVisualStyleBackColor = true;
            this.ParseTeamsUsersButton.Click += new System.EventHandler(this.TeamsUsersParsingProcess);
            // 
            // LogsListBox
            // 
            this.LogsListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LogsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogsListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.LogsListBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LogsListBox.FormattingEnabled = true;
            this.LogsListBox.HorizontalScrollbar = true;
            this.LogsListBox.ItemHeight = 15;
            this.LogsListBox.Location = new System.Drawing.Point(283, 0);
            this.LogsListBox.Name = "LogsListBox";
            this.LogsListBox.Size = new System.Drawing.Size(1101, 303);
            this.LogsListBox.TabIndex = 3;
            this.LogsListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.LogsListBox_DrawItem);
            this.LogsListBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.LogsListBox_MeasureItem);
            // 
            // bottomPanel
            // 
            this.bottomPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bottomPanel.Controls.Add(this.ParseTeamsUsersButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(283, 303);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(1101, 58);
            this.bottomPanel.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 361);
            this.Controls.Add(this.LogsListBox);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.LeftPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1400, 400);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.serviceInfoPanel.ResumeLayout(false);
            this.serviceInfoPanel.PerformLayout();
            this.serviceStatusContextMenuStrip.ResumeLayout(false);
            this.LeftPanel.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel serviceInfoPanel;
        private System.Windows.Forms.Label serviceStatusLabel;
        private System.Windows.Forms.Label serviceStaticLabel;
        private System.Windows.Forms.Button uninstallServiceButton;
        private System.Windows.Forms.CheckBox AdministratorCheckBox;
        private System.Windows.Forms.Button installServiceButton;
        private System.Windows.Forms.ContextMenuStrip serviceStatusContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyInstallCommandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.CheckBox closeWindowCheckBox;
        private System.Windows.Forms.Label commandLineoptionsStaticLabel;
        private System.Windows.Forms.Panel LeftPanel;
        private System.Windows.Forms.ListBox LogsListBox;
        private System.Windows.Forms.ToolStripMenuItem checkNowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopstartAutoCheckToolStripMenuItem;
        private System.Windows.Forms.Button ParseTeamsUsersButton;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Label label1;
        private NumericLabel numericLabel1;
        private NumericLabel numericLabel12;
        private System.Windows.Forms.Label label12;
        private NumericLabel numericLabel11;
        private System.Windows.Forms.Label label11;
        private NumericLabel numericLabel10;
        private System.Windows.Forms.Label label10;
        private NumericLabel numericLabel9;
        private System.Windows.Forms.Label label9;
        private NumericLabel numericLabel8;
        private System.Windows.Forms.Label label8;
        private NumericLabel numericLabel7;
        private System.Windows.Forms.Label label7;
        private NumericLabel numericLabel6;
        private System.Windows.Forms.Label label6;
        private NumericLabel numericLabel5;
        private System.Windows.Forms.Label label5;
        private NumericLabel numericLabel4;
        private System.Windows.Forms.Label label4;
        private NumericLabel numericLabel3;
        private System.Windows.Forms.Label label3;
        private NumericLabel numericLabel2;
        private System.Windows.Forms.Label label2;
    }
}

