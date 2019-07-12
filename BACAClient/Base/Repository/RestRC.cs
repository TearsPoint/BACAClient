using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel;
using DataService.ServiceModel;
using Base;
using System.Collections.Concurrent;
using fastJSON;
using JetSun.Base.Infrastructure;

namespace Base.Repository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class RestRC<T>
        where T : class
    {
        private static T _repository;
        /// <summary>
        /// 
        /// </summary>
        public static T Repository
        {
            get
            {
                if (_repository == null)
                {
                    _repository = typeof(T).Intance() as T;
                }
                return _repository;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class RepositoryHelper
    {
        static IDictionary<Type, object> _repository = new ConcurrentDictionary<Type, object>();
        public static T GetRepository<T>() where T : class
        {
            if (!_repository.ContainsKey(typeof(T)))
                _repository[typeof(T)] = RestRC<T>.Repository;
            return _repository[typeof(T)] as T;
        }


    }
}
