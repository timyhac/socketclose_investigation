using libplctag.NativeImport;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UWP
{
    public class PlcViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;


        public string Gateway { get; set; } = "192.168.0.10";
        public string Path { get; set; } = "1,0";
        public string Name { get; set; } = "MY_DINT[0]";
        public int Timeout { get; set; } = 1000;

        public bool TagValid { get; set; }

        public ICommand RunCommand { get; set; }


        public int Value { get; set; }


        public event EventHandler<Exception> OnError;


        public PlcViewModel()
        {
            RunCommand = new RelayCommand(Run);

            plctag.ForceExtractLibrary = false;
        }

        void Run()
        {
            try
            {

                var myLogger = new plctag.log_callback_func(MyLogger);
                var statusAfterLoggerRegistration = plctag.plc_tag_register_logger(myLogger);

                plctag.plc_tag_set_debug_level(2);

                var tagHandle = plctag.plc_tag_create("protocol=ab_eip&gateway=192.168.0.10&path=1,0&plc=LGX&elem_size=4&elem_count=1&name=MY_DINT", 1000);

                while (plctag.plc_tag_status(tagHandle) == 1)
                {
                    Thread.Sleep(100);
                }
                var statusBeforeRead = plctag.plc_tag_status(tagHandle);
                if (statusBeforeRead != 0)
                {
                    Debug.WriteLine($"Something went wrong {statusBeforeRead}");
                }


                var myCallback = new plctag.callback_func(MyCallback);
                var statusAfterRegistration = plctag.plc_tag_register_callback(tagHandle, myCallback);
                if (statusAfterRegistration != 0)
                {
                    Debug.WriteLine($"Something went wrong {statusAfterRegistration}");
                }

                plctag.plc_tag_read(tagHandle, 1000);
                while (plctag.plc_tag_status(tagHandle) == 1)
                {
                    Thread.Sleep(100);
                }
                var statusAfterRead = plctag.plc_tag_status(tagHandle);
                if (statusAfterRead != 0)
                {
                    Debug.WriteLine($"Something went wrong {statusAfterRead}");
                }

                Value = (int)plctag.plc_tag_get_uint32(tagHandle, 0);

                // Problem occurs here
                var destroyResult = plctag.plc_tag_destroy(tagHandle);

            }
            catch (Exception e)
            {
                OnError?.Invoke(this, e);
            }
        }

        public static void MyCallback(int tag_id, int event_id, int status)
        {
            Debug.WriteLine($"Tag Id: {tag_id}    Event Id: {event_id}    Status: {status}");
        }

        public static void MyLogger(int tag_id, int debug_level, string message)
        {
            Debug.Write($"Tag Id: {tag_id}    Debug Level: {debug_level}    Message: {message}");
        }

    }
}
