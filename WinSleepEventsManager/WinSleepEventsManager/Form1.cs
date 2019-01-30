using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WinSleepEventsManager {
    public partial class Form1 : Form {
        private const String separationString = "####################################################################\r\n";
        private Boolean continueReadSystem;
        private Boolean continueReadApp;
        private Boolean continueReadSecurity;
        public Form1() {
            InitializeComponent();
            continueReadSystem   = false;
            continueReadApp      = false;
            continueReadSecurity = false;
        }

        private void button1_Click(object sender, EventArgs e) {
            if (continueReadSystem) {
                button1.Text = "Start";
                continueReadSystem = false;
            } else {
                button1.Text = "Stop";
                continueReadSystem = true;
                Thread thread = new Thread(new ThreadStart(readSystemThread));
                thread.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            if (continueReadApp) {
                button2.Text = "Start";
                continueReadApp = false;
            } else {
                button2.Text = "Stop";
                continueReadApp = true;
                Thread thread = new Thread(new ThreadStart(readAppThread));
                thread.Start();
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            //check if admin
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!isElevated) {
                MessageBox.Show(this, "Please restart the app with admin rights !", "Needs elevation !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                ProcessStartInfo elevated = new ProcessStartInfo(System.Reflection.Assembly.GetEntryAssembly().Location, "run2");
                elevated.UseShellExecute = true;
                elevated.Verb = "runas";
                try {
                    Process.Start(elevated);
                    Application.Exit();
                } catch (Exception ex) {
                    MessageBox.Show(this, "You dismissed the prompt...", "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            } else {
                if (continueReadSecurity) {
                    button3.Text = "Start";
                    continueReadSecurity = false;
                } else {
                    button3.Text = "Stop";
                    continueReadSecurity = true;
                    Thread thread = new Thread(new ThreadStart(readSecurityThread));
                    thread.Start();
                }
            }
        }
        public void readSystemThread() {
            this.Invoke((MethodInvoker)delegate {
                textBox1.Clear();
            });
            string eventLogName = "System";

            EventLog eventLog = new EventLog();
            eventLog.Log = eventLogName;

            int i = 1;

            if (checkBox11.Checked) {
                //TODO

            }
            //eventLog.Entries.Cast<EventLogEntry>().Reverse()
            foreach (EventLogEntry log in eventLog.Entries) {
                if (!continueReadSystem)
                    break;
                this.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    label1.Text = i + " / " + eventLog.Entries.Count;
                });
                //Out of sleep
                if (checkBox1.Checked && log.EventID == 1 && log.Source.Contains("Power-Troubleshooter")) {
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.Text += String.Format("{0} Exits sleep, reason : {1}\r\n", log.TimeGenerated, "asf");
                    });
                }
                //Enter sleep
                if (checkBox2.Checked && log.EventID == 42 && log.Source.Contains("Kernel-Power")) {
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.Text += String.Format("{0} Enter sleep\r\n", log.TimeGenerated);
                    });
                }
                //Crash/BSOD   Windows Error Reporting ?? BugCheck ok
                if (checkBox3.Checked && log.EventID == 1001 && log.Source.Contains("BugCheck")) {
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.Text += separationString;
                        textBox1.Text += String.Format("{0} Crash/BSOD : \r\n{1}\r\n\r\n", log.TimeGenerated, log.Message);
                    });
                }
                //Service started
                if (checkBox4.Checked && log.EventID == 6005 && log.Source.Contains("EventLog")) {
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.Text += String.Format("{0} EventVwr started (startup)\r\n", log.TimeGenerated);
                    });
                }
                //Service stopped
                if (checkBox5.Checked && log.EventID == 6006 && log.Source.Contains("EventLog")) {
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.Text += String.Format("{0} EventVwr stopped (shutdown)\r\n", log.TimeGenerated);
                    });
                }
                //Forced shutdown
                if (checkBox6.Checked && log.EventID == 6008 && log.Source.Contains("EventLog")) {
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.Text += String.Format("{0} Forced shutdown\r\n", log.TimeGenerated);
                    });
                }
                i++;
                if (i >= 40000)
                    break;
            }
            continueReadSystem = false;
            this.Invoke((MethodInvoker)delegate {
                button1.Text = "Start";
            });
        }
        public void readAppThread() {
            this.Invoke((MethodInvoker)delegate {
                textBox1.Clear();
            });
            string eventLogName = "Application";

            EventLog eventLog = new EventLog();
            eventLog.Log = eventLogName;

            int i = 1;
            //eventLog.Entries.Cast<EventLogEntry>().Reverse()
            foreach (EventLogEntry log in eventLog.Entries) {
                if (!continueReadApp)
                    break;
                this.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    label1.Text = i + " / " + eventLog.Entries.Count;
                });
                //chkdsk during boot
                if (checkBox7.Checked && log.EventID == 1001 && log.Source.Contains("Wininit")) {
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.Text += separationString;
                        textBox1.Text += String.Format("{0} Chkdsk during boot :\r\n{1}\r\n\r\n", log.TimeGenerated, log.Message);
                    });
                }
                //chkdsk during session 26212? 26214 ok
                if (log.Source.Contains("Chkdsk")) {
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.Text += separationString;
                        textBox1.Text += String.Format("{0} Chkdsk during session :\r\n{1}\r\n\r\n", log.TimeGenerated, log.Message);
                    });
                }
                i++;
                if (i >= 40000)
                    break;
            }
            continueReadApp = false;
            this.Invoke((MethodInvoker)delegate {
                button2.Text = "Start";
            });
        }
        public void readSecurityThread() {
            this.Invoke((MethodInvoker)delegate {
                textBox1.Clear();
            });
            string eventLogName = "Security";

            EventLog eventLog = new EventLog();
            eventLog.Log = eventLogName;

            int i = 1;
            //eventLog.Entries.Cast<EventLogEntry>().Reverse()
            foreach (EventLogEntry log in eventLog.Entries) {
                if (!continueReadSecurity)
                    break;
                this.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    label1.Text = i + " / " + eventLog.Entries.Count;
                });
                //Windows locked
                if (checkBox9.Checked && log.EventID == 4800 && log.Source.Contains("Auditing")) {
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.Text += String.Format("{0} Windows locked\r\n", log.TimeGenerated);
                    });
                }
                //Windos unlocked
                if (checkBox10.Checked && log.EventID == 4801 && log.Source.Contains("Auditing")) {
                    this.Invoke((MethodInvoker)delegate {
                        textBox1.Text += String.Format("{0} Windows unlocked\r\n", log.TimeGenerated);
                    });
                }
                i++;
                if (i >= 40000)
                    break;
            }
            continueReadSecurity = false;
            this.Invoke((MethodInvoker)delegate {
                button3.Text = "Start";
            });
        }
    }
}
