using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shadowrun_Launcher
{
    internal class Helper
    {
        private string DecimalToHexFormat(int decimalValue)
        {
            string hexValue = decimalValue.ToString("X"); // Convert to hex
            char[] hexChars = hexValue.ToCharArray(); // Convert to char array
            Array.Reverse(hexChars); // Reverse the array

            string reversedHex = new string(hexChars); // Convert back to string
            string formattedHex = string.Join(",", SplitIntoChunks(reversedHex, 2)); // Group into pairs and join with commas

            return formattedHex;
        }

        private string[] SplitIntoChunks(string str, int chunkSize)
        {
            int numChunks = (int)Math.Ceiling((double)str.Length / chunkSize);
            string[] chunks = new string[numChunks];

            for (int i = 0; i < numChunks; i++)
            {
                int startIndex = i * chunkSize;
                int length = Math.Min(chunkSize, str.Length - startIndex);
                chunks[i] = str.Substring(startIndex, length);
            }

            return chunks;
        }
        private bool CheckPcidInRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\SOFTWARE\Microsoft\XLive", false))
                {
                    if (key != null)
                    {
                        object pcidValue = key.GetValue("PCID");
                        return pcidValue != null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error accessing registry: {e}");
            }
            return false;
        }

        private string GetPcidFromRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\SOFTWARE\Microsoft\XLive", false))
                {
                    if (key != null)
                    {
                        object pcidValue = key.GetValue("PCID");
                        return pcidValue?.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error accessing registry: {e}");
            }
            return null;
        }
        private bool UawPcidChangeExists()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\SOFTWARE\Microsoft\XLive", false))
                {
                    if (key != null)
                    {
                        object uawPcidBackup = key.GetValue("UAWPCIDBACKUP");
                        return uawPcidBackup != null;
                    }
                }
            }
            catch (Exception)
            {
                // If an exception occurs, it means the key does not exist
            }
            return false;
        }
    }
}
