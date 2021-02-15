using libplctag.NativeImport;
using System.Threading;

namespace Example
{
    public class MethodThatThrows
    {

        public string Log { get; internal set; }

        public int Run()
        {

            Log = "";

            plctag.ForceExtractLibrary = false;

            var myLogger = new plctag.log_callback_func(logger);
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
                AddToLog($"Something went wrong {statusBeforeRead}");
            }


            var myEventCallback = new plctag.callback_func(eventCallback);
            var statusAfterRegistration = plctag.plc_tag_register_callback(tagHandle, myEventCallback);
            if (statusAfterRegistration != 0)
            {
                AddToLog($"Something went wrong {statusAfterRegistration}");
            }

            plctag.plc_tag_read(tagHandle, 1000);
            while (plctag.plc_tag_status(tagHandle) == 1)
            {
                Thread.Sleep(100);
            }
            var statusAfterRead = plctag.plc_tag_status(tagHandle);
            if (statusAfterRead != 0)
            {
                AddToLog($"Something went wrong {statusAfterRead}");
            }

            var value = (int)plctag.plc_tag_get_uint32(tagHandle, 0);

            // Problem occurs here
            var destroyResult = plctag.plc_tag_destroy(tagHandle);

            return value;
        }

        void eventCallback(int tag_id, int event_id, int status)
        {
            AddToLog($"Tag Id: {tag_id}    Event Id: {event_id}    Status: {status}");
        }

        void logger(int tag_id, int debug_level, string message)
        {
            AddToLog($"Tag Id: {tag_id}    Debug Level: {debug_level}    Message: {message}");
        }

        void AddToLog(string message)
        {
            Log += message + "\n";
        }

    }

}
