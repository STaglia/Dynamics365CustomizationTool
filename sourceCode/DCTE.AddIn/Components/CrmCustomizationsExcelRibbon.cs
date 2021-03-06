﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System.Windows.Forms;
using Microsoft.Xrm.Sdk.Discovery;
using System.ServiceModel.Description;
using System.Net;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using DynamicsCRMCustomizationToolForExcel.Controller;
using DynamicsCRMCustomizationToolForExcel.Model;
using System.Threading.Tasks;
using DynamicsCRMCustomizationToolForExcel.AddIn.Components;

namespace DynamicsCRMCustomizationToolForExcel.AddIn
{
    public partial class CrmCustomizationsRibbon
    {

        public void enableRibbonButtons()
        {
            btnPublishAll.Enabled = true;
            btnUpdateSheet.Enabled = true;
            btnExportChanges.Enabled = true;
            btnSynchronizeEntity.Enabled = true;
            btnGetEntitiesList.Enabled = true;
        }


        private void btnConnect_Click(object sender, RibbonControlEventArgs e)
        {
            CRMLoginForm ctrl = new CRMLoginForm();

            // Wire event to login response. 
            ctrl.ConnectionToCrmCompleted += ctrl_ConnectionToCrmCompleted;

            // Show the login control. 
            ctrl.ShowDialog();

            // Handle the returned CRM connection object.
            // On successful connection, display the CRM version and connected org name 
            if (ctrl.CrmConnectionMgr != null && ctrl.CrmConnectionMgr.CrmSvc != null && ctrl.CrmConnectionMgr.CrmSvc.IsReady)
            {
                MessageBox.Show("Connected to CRM! Version: " + ctrl.CrmConnectionMgr.CrmSvc.ConnectedOrgVersion.ToString() +
                " Org: " + ctrl.CrmConnectionMgr.CrmSvc.ConnectedOrgUniqueName, "Connection Status");

                // Perform your actions here
            }
            else
            {
                MessageBox.Show("Cannot connect; try again!", "Connection Status");
            }
        }

        private async void ctrl_ConnectionToCrmCompleted(object sender, EventArgs e)
        {
            if (sender is CRMLoginForm)
            {
               ((CRMLoginForm)sender).Close();
               Loading loading = new Loading();
               loading.Show();
               await Task.Run(() =>
               {
                   GlobalApplicationData.Instance.connectionInProgress = true;
                   OrganizationServiceProxy organizationProxy = ((CRMLoginForm)sender).CrmConnectionMgr.CrmSvc.OrganizationServiceProxy;
                   GlobalOperations.Instance.CRMOpHelper.Service = organizationProxy;
                   GlobalOperations.Instance.LoadOperations();
               });
               loading.Close();
               Globals.DynamicsCRMExcelAddIn.MainPanel.Visible = true;
               ((MainPanelEnityList)Globals.DynamicsCRMExcelAddIn.MainPanel.Control).FillEntitiesList();
               enableRibbonButtons();
               GlobalApplicationData.Instance.connectionInProgress = false;
            }
        }


        private void btnExportChanges_Click(object sender, RibbonControlEventArgs e)
        {
            if (GlobalOperations.Instance.ExcelOperations.IsEditing())
            {
                MessageBox.Show("Plesea exit from edit-mode", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ConfirmCustomization confirm = new ConfirmCustomization();
            confirm.ShowDialog();
        }


        private void btnUpdateSheet_Click(object sender, RibbonControlEventArgs e)
        {
            if ( GlobalOperations.Instance.ExcelOperations.IsEditing())
            {
                MessageBox.Show("Plesea exit from edit-mode", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            GlobalOperations.Instance.RefreshCurrentSheet();
        }

      
        private void btnSynchronizeEntity_Click(object sender, RibbonControlEventArgs e)
        {
            //if (Globals.CrmAddIn.excelHelper.IsEditing())
            //{
            //    MessageBox.Show("Plesea exit from edit-mode", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //Worksheet sheet = Globals.CrmAddIn.excelHelper.getCurrentSheet();
            //ExcelSheetInfo.ExcelSheetType type;
            //string name;
            //string orgprefix;
            //int language;
            //if (!Globals.CrmAddIn.excelHelper.readSettingRow(sheet, out name, out type, out orgprefix, out language)) return;
            //if (type == ExcelSheetInfo.ExcelSheetType.attribute)
            //{
            //    EntityMetadata currentEntity = Globals.CrmAddIn.crmOpHelper.RetriveEntityAtrribute(name);
            //    if (currentEntity != null)
            //    {
            //        GlobalApplicationData.Instance.eSheetsInfomation.addSheetAndSetAsCurrent(new AttributeExcelSheetsInfo(ExcelSheetInfo.ExcelSheetType.attribute, sheet, currentEntity), name);
            //        GlobalApplicationData.Instance.eSheetsInfomation.getCurrentSheet().orgPrefix = orgprefix;
            //        GlobalApplicationData.Instance.eSheetsInfomation.getCurrentSheet().language = language;
            //    }
            //    else
            //    {
            //        MessageBox.Show("Entity not Found Impossible To syncronize", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //else if (type == ExcelSheetInfo.ExcelSheetType.optionSet)
            //{
            //    // to be implemented
            //}
        }

        private void btnGetEntitiesList_Click(object sender, RibbonControlEventArgs e)
        {
            //if (GlobalOperations.Instance.ExcelOperations.IsEditing())
            //{
            //    MessageBox.Show("Plesea exit from edit-mode", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            ////GlobalOperations.CreatenNewEntitySheet();
            //if (Globals.DynamicsCRMExcelAddIn.MainPanel.Visible == false)
            //{
            //    Globals.CrmAddIn.TaskPane.Visible = true;
            //    ((ActionPanelEntityList)Globals.CrmAddIn.TaskPane.Control).showLoading(true);
            //    GlobalOperations.LoadOperations();
            //    ((ActionPanelEntityList)Globals.CrmAddIn.TaskPane.Control).FillEntitiesList();
            //}
        }

        private void btnPublishAll_Click(object sender, RibbonControlEventArgs e)
        {
            GlobalOperations.Instance.CRMOpHelper.publishRequest();
        }
    }
}
