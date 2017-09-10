using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    class DonationFrameThread
    {
        private Thread _showDonationThread;
        private frmDonation _frmDonation;

        public DonationFrameThread()
        {
            using (_frmDonation = new frmDonation())
            {
                _showDonationThread = new Thread(this.run);
                _showDonationThread.IsBackground = true;
            }
        }
        public void start()
        {
            _showDonationThread.Start();
        }

        public void stop()
        {
            if (_frmDonation.InvokeRequired)
            {
                ;
                MethodInvoker methodInvoker = new MethodInvoker(this.stop);
                IAsyncResult result = _frmDonation.BeginInvoke(methodInvoker);
                _frmDonation.EndInvoke(result);
            }
            else
            {
                try
                {
                    _frmDonation.Close();
                    //_showDonationThread.Abort();
                }
                catch (Exception)
                {
                }
            }
        }
        private void run()
        {            
            _frmDonation.ShowDialog();
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
