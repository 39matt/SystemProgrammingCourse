using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMapAPI
{
    internal class WeatherCache
    {
        private static readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<int, string> cache = new Dictionary<int, string>();

        public static string ReadFromCache(int key)
        {
            cacheLock.EnterReadLock();
            try
            {
                if (cache.TryGetValue(key, out string value))
                {
                    return value;
                }
                else
                {
                    throw new KeyNotFoundException($"Vrednost za kljuc {key} nije pronadjena!");
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                throw;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }
        public static void WriteToCache(int key, string value)
        {
            cacheLock.EnterWriteLock();
            try
            {
                cache[key] = value;
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                throw;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }
        public static bool WriteToCacheWithTimeout(int key, string value, int timeout)
        {
            if (cacheLock.TryEnterWriteLock(timeout))
            {
                try
                {
                    cache[key] = value;
                    return true;
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    return false;
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            else
            {
                Console.WriteLine("Lock nije pribavljen u odgovarajucem intervalu.");
                return false;
            }
        }
        public static void RemoveFromCache(int key)
        {
            cacheLock.EnterWriteLock();
            try
            {
                if (cache.ContainsKey(key))
                {
                    cache.Remove(key);
                }
                else
                {
                    throw new KeyNotFoundException($"Vrednost za kljuc {key} nije pronadjena!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }
        public static void CacheInfos(List<WeatherInfo> infos)
        {
            int count = 0;
            foreach (var info in infos)
            {
                var content = $"City: {info.City}\nWeather: {info.Weather}\nDescription: {info.Description}\n";
                WriteToCache(count++, content);
            }
        }
    }
}
