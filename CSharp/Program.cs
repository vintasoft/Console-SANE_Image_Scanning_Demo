using System;
using System.IO;
using Vintasoft.Sane;

namespace SaneConsoleDemo
{
    public class Program
    {

        public static int Main(string[] args)
        {
            using (SaneLocalDeviceManager deviceManager = new SaneLocalDeviceManager())
            {
                deviceManager.Open();

                int deviceCount = deviceManager.Devices.Count;
                if (deviceCount == 0)
                {
                    Console.WriteLine("Devices are not found.");
                    return -1;
                }

                int deviceIndex = SelectDevice(deviceManager);
                if (deviceIndex == 0)
                    return -2;

                SaneLocalDevice device = deviceManager.Devices[deviceIndex - 1];
                if (device != null)
                {
                    device.Open();

                    string scanSource = SelectSaneScanSource(device);
                    if (scanSource != null)
                        device.ScanSource = scanSource;

                    string scanMode = SelectSaneScanMode(device);
                    if (scanMode != null)
                        device.ScanMode = scanMode;

                    int scanResolution = SelectSaneScanResolution(device);
                    if (scanResolution != 0)
                        device.ScanResolution = scanResolution;

                    int imageIndex = 0;
                    SaneAcquiredImage acquiredImage;
                    do
                    {
                        try
                        {
                            acquiredImage = device.AcquireImageSync();
                            if (acquiredImage != null)
                            {
                                string filename = "scannedImage.png";
                                if (File.Exists(filename))
                                    File.Delete(filename);

                                acquiredImage.Save(filename);

                                Console.WriteLine(string.Format("Image{0} is saved.", imageIndex++));
                            }
                            else
                            {
                                Console.WriteLine("Scan is completed.");
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(string.Format("Scan is failed: {0}", ex.Message));
                            break;
                        }
                    }
                    while (device.HasMoreImagesToScan);

                    device.Close();
                }

                deviceManager.Close();
            }

            return 0;
        }

        static int SelectDevice(SaneLocalDeviceManager deviceManager)
        {
            int deviceCount = deviceManager.Devices.Count;

            Console.WriteLine("Device list:");
            for (int i = 0; i < deviceCount; i++)
            {
                Console.WriteLine(string.Format("{0}. {1}", i + 1, deviceManager.Devices[i].Name));
            }

            int deviceIndex = -1;
            while (deviceIndex < 0 || deviceIndex > deviceCount)
            {
                Console.Write(string.Format("Please select device by entering the device number from '1' to '{0}' or press '0' to cancel: ", deviceCount));
                deviceIndex = Console.ReadKey().KeyChar - '0';
                Console.WriteLine();
            }
            Console.WriteLine();

            return deviceIndex;
        }

        static string SelectSaneScanSource(SaneLocalDevice device)
        {
            string[] supportedScanSources = device.GetSupportedScanSources();
            Console.WriteLine("Scan sources:");
            for (int i = 0; i < supportedScanSources.Length; i++)
            {
                Console.WriteLine(string.Format("{0}. {1}", i + 1, supportedScanSources[i]));
            }

            int scanSourceIndex = -1;
            while (scanSourceIndex < 0 || scanSourceIndex > supportedScanSources.Length)
            {
                Console.Write(string.Format("Please select scan source by entering the number from '1' to '{0}' or press '0' to cancel: ", supportedScanSources.Length));
                scanSourceIndex = Console.ReadKey().KeyChar - '0';
                Console.WriteLine();
            }
            Console.WriteLine();

            if (scanSourceIndex == 0)
                return null;

            return supportedScanSources[scanSourceIndex - 1];
        }

        static string SelectSaneScanMode(SaneLocalDevice device)
        {
            string[] supportedScanModes = device.GetSupportedScanModes();
            Console.WriteLine("Scan modes:");
            for (int i = 0; i < supportedScanModes.Length; i++)
            {
                Console.WriteLine(string.Format("{0}. {1}", i + 1, supportedScanModes[i]));
            }

            int scanModeIndex = -1;
            while (scanModeIndex < 0 || scanModeIndex > supportedScanModes.Length)
            {
                Console.Write(string.Format("Please select scan mode by entering the number from '1' to '{0}' or press '0' to cancel: ", supportedScanModes.Length));
                scanModeIndex = Console.ReadKey().KeyChar - '0';
                Console.WriteLine();
            }
            Console.WriteLine();

            if (scanModeIndex == 0)
                return null;

            return supportedScanModes[scanModeIndex - 1];
        }

        static int SelectSaneScanResolution(SaneLocalDevice device)
        {
            int[] supportedScanResolutions = device.GetSupportedScanResolutions();
            Console.WriteLine("Scan resolutions:");
            for (int i = 0; i < supportedScanResolutions.Length; i++)
            {
                Console.WriteLine(string.Format("{0}. {1}", i + 1, supportedScanResolutions[i]));
            }

            int scanResolutionIndex = -1;
            while (scanResolutionIndex < 0 || scanResolutionIndex > supportedScanResolutions.Length)
            {
                Console.Write(string.Format("Please select scan resolution by entering the number from '1' to '{0}' or press '0' to cancel: ", supportedScanResolutions.Length));
                scanResolutionIndex = Console.ReadKey().KeyChar - '0';
                Console.WriteLine();
            }
            Console.WriteLine();

            if (scanResolutionIndex == 0)
                return 0;

            return supportedScanResolutions[scanResolutionIndex - 1];
        }

    }
}