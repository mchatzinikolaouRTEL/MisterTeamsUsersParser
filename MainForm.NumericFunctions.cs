using RtelLibrary.Enums;

namespace MisterProtypoParser
{
    partial class MainForm
    {
        private void HideAllNumericLabels()
        {
            label1.Visible = false;
            numericLabel1.Visible = false;
            label2.Visible = false;
            numericLabel2.Visible = false;
            label3.Visible = false;
            numericLabel3.Visible = false;
            label4.Visible = false;
            numericLabel4.Visible = false;
            label5.Visible = false;
            numericLabel5.Visible = false;
            label6.Visible = false;
            numericLabel6.Visible = false;
            label7.Visible = false;
            numericLabel7.Visible = false;
            label8.Visible = false;
            numericLabel8.Visible = false;
            label8.Visible = false;
            numericLabel9.Visible = false;
            label10.Visible = false;
            numericLabel10.Visible = false;
            label11.Visible = false;
            numericLabel11.Visible = false;
            label12.Visible = false;
            numericLabel12.Visible = false;
        }
        private void InitializeNumericLabels(SysApplicationProcess sysApplicationProcess)
        {
            HideAllNumericLabels();
            switch (sysApplicationProcess)
            {
                case SysApplicationProcess.ReplicateLDAPData:
                    label1.Text = "Total Records:";
                    label1.Visible = true;
                    numericLabel1.NumericText = 0;
                    numericLabel1.Visible = true;
                    label2.Text = "Inserted Records:";
                    label2.Visible = true;
                    numericLabel2.NumericText = 0;
                    numericLabel2.Visible = true;
                    label3.Text = "Updated Records:";
                    label3.Visible = true;
                    numericLabel3.NumericText = 0;
                    numericLabel3.Visible = true;
                    label4.Text = "Error Records:";
                    label4.Visible = true;
                    numericLabel4.NumericText = 0;
                    numericLabel4.Visible = true;                    
                    break;
            }
        }

        public void AddTotalRecords(SysApplicationProcess sysApplicationProcess, int i)
        {
            switch (sysApplicationProcess)
            {
                case SysApplicationProcess.ReplicateLDAPData:
                    numericLabel1.NumericText += i;
                    break;
            }            
        }

        public void AddInsertedRecords(SysApplicationProcess sysApplicationProcess, int i)
        {
            switch (sysApplicationProcess)
            {
                case SysApplicationProcess.ReplicateLDAPData:
                    numericLabel2.NumericText += i;
                    break;
            }
        }

        public void AddUpdatedRecords(SysApplicationProcess sysApplicationProcess, int i)
        {
            switch (sysApplicationProcess)
            {
                case SysApplicationProcess.ReplicateLDAPData:
                    numericLabel3.NumericText += i;
                    break;
            }
        }

        public void AddErrorRecords(SysApplicationProcess sysApplicationProcess, int i)
        {
            switch (sysApplicationProcess)
            {
                case SysApplicationProcess.ReplicateLDAPData:
                    numericLabel4.NumericText += i;
                    break;
            }
        }
    }
}
