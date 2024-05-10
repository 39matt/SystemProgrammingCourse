using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMapAPI
{
    internal class Cache
    {
        private static ReaderWriterLockSlim _lock = new();
        private static Dictionary<string, List<WeatherInfo>> _cache = new();

        public bool ContainsKey(string key)
        {
            _lock.EnterReadLock();
            try
            {
                return _cache.ContainsKey(key);
            }
            catch(Exception e)
            {
                throw new Exception($"Error checking if key exists in cache: {e.Message}");
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void AddToCache(string key, List<WeatherInfo> value)
        {
            _lock.EnterWriteLock();
            try
            {
                if (_cache.ContainsKey(key))
                {
                    _cache[key] = value;
                }
                else
                {
                    _cache.Add(key, value);
                }

            }
            catch(Exception e)
            {
                throw new Exception($"Error adding to cache: {e.Message}");
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public List<WeatherInfo> GetFromCache(string key)
        {

            _lock.EnterReadLock();
            try
            {
                if(_cache.TryGetValue(key, out List<WeatherInfo> value) && value != null)
                {
                    return value;
                }
                else throw new Exception($"Key {key} not found in cache");
            }
            catch(Exception e)
            {
                throw new Exception($"Error getting from cache: {e.Message}");
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
