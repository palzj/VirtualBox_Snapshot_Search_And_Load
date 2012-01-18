﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Diagnostics;

namespace VirtualBoxForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //SECTION 1. Create a DOM Document and load the XML data into it.
                XmlDocument dom = new XmlDocument();
                dom.Load("Virtual Machine.xml");

                //SECTION 2. Initialize the TreeView control.
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(new TreeNode(dom.DocumentElement.Name));
                TreeNode tNode = new TreeNode();
                tNode = treeView1.Nodes[0];

                //SECTION 3. Populate the TreeView with the DOM nodes.
                AddNode(dom.DocumentElement, tNode);
                treeView1.ExpandAll();
            }
            catch (XmlException xmlEx)
            {
                MessageBox.Show(xmlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i;

            // Loop through the XML nodes until the leaf is reached.
            // Add the nodes to the TreeView during the looping process.
            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;
                for (i = 0; i <= nodeList.Count - 1; i++)
                {
                    xNode = inXmlNode.ChildNodes[i];
                    inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    AddNode(xNode, tNode);
                }
            }
            else
            {
                // Here you need to pull the data from the XmlNode based on the
                // type of node, whether attribute values are required, and so forth.
                inTreeNode.Text = (inXmlNode.OuterXml).Trim();
            }
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            ClearBackColor();
        }

        private void ClearBackColor()
        {
            TreeNodeCollection nodes = treeView1.Nodes;
            foreach (TreeNode n in nodes)
            {
                ClearRecursive(n);
            }
        }

        private void ClearRecursive(TreeNode treeNode)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                tn.BackColor = Color.White;
                ClearRecursive(tn);
            }
        }

        private void cmdNodeSearch_Click(object sender, EventArgs e)
        {
            ClearBackColor();
            FindByText();
        }

        private void FindByText()
        {
            TreeNodeCollection nodes = treeView1.Nodes;
            foreach (TreeNode n in nodes)
            {
                FindRecursive(n);
            }
        }

        private void FindRecursive(TreeNode treeNode)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                //if the text properties match, color the item
                if (tn.Text == this.txtNodeSearch.Text)
                    tn.BackColor = Color.Yellow;
                FindRecursive(tn);
            }
        }

        private void showNodes_Click(object sender, EventArgs e)
        {
            //disable redrawing of treeView1 to prevent flickering while changes are made.
            treeView1.BeginUpdate();
            //collapse all nodes of treeView1.
            treeView1.CollapseAll();
            //add the checkForCheckedChildren event handler to the BeforeExpand event.
            treeView1.BeforeExpand += checkForCheckedChildren;
            //expand all nodes of treeView1. Nodes without checked children are prevented from expanding by the checkForCheckedChildren event handler.
            treeView1.ExpandAll();
            //remove the checkForCheckedChildren event handler from the BeforeExpand event so manual node expansion will work correctly.
            treeView1.BeforeExpand -= checkForCheckedChildren;
            //enable redrawing of treeView1.
            treeView1.EndUpdate();
        }

        private void checkForCheckedChildren(object sender, TreeViewCancelEventArgs e)
        {
            if (!HasCheckedChildrenNodes(e.Node)) e.Cancel = true;
        }

        private bool HasCheckedChildrenNodes(TreeNode node)
        {
            if (node.Nodes.Count == 0) return false;
            foreach (TreeNode childNode in node.Nodes)
            {
                if (childNode.Checked) return true;
                if (HasCheckedChildrenNodes(childNode)) return true;
            }
            return false;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            lstbox.Items.Add(treeView1.SelectedNode);
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            txtNodeSearch.Focus();
            cmdNodeSearch.Enabled = false;
        }

        private void txtNodeSearch_TextChanged(object sender, EventArgs e)
        {
            cmdNodeSearch.Enabled = true;
        }

        private void lstbox_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process vbproc = new System.Diagnostics.Process();
            vbproc.StartInfo.FileName = @"C:\Program Files\Oracle\VirtualBox\VBoxManage.exe";
            vbproc.StartInfo.Arguments = "cmd.exe";
            vbproc.StartInfo.WorkingDirectory = @"C:\Documents and Settings\szorilla\VirtualBox VMs\Virtual Machine\Snapshots\2012-01-04T10-48-09-841877700Z.sav";
            vbproc.Start();
        }
    }
}