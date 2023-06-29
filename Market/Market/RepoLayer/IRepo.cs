using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.RepoLayer
{
    public interface IRepo<T>
    {
        List<T> GetAll();
        T GetById(int id);
        void Add(T item);
        void Update(T item);
        void Delete(int id);
        Boolean ContainsID(int id);
        Boolean ContainsValue(T item);
        void ResetDomainData();
        void Clear();
    }
}
