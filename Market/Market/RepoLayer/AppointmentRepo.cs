using Market.DataLayer;
using Market.DataLayer.DTOs;
using Market.DomainLayer;
using Market.ServiceLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.RepoLayer
{
    public class AppointmentRepo
    {
        //<appointmentId, Appointment>
        private static Dictionary<int, Appointment> _appointments;
        private static AppointmentRepo _appointmentRepo = null;

        private AppointmentRepo()
        {
            _appointments = new Dictionary<int, Appointment>();
        }
        public static AppointmentRepo GetInstance()
        {
            if (_appointmentRepo == null)
                _appointmentRepo = new AppointmentRepo();
            return _appointmentRepo;
        }
        public void Add(Appointment item)
        {
            int unicId = AppointmentUnicId(item);
            if (_appointments.ContainsKey(unicId))
                throw new Exception("User already have appointment in this shop");
            _appointments[unicId] = item;

            MarketContext.GetInstance().Appointments.Add(new AppointmentDTO(item));
        }
        private int AppointmentUnicId(Appointment item)
        {
            return int.Parse($"{item.Member.Id}{item.Shop.Id}");
        }
        private int AppointmentUnicId(AppointmentDTO item)
        {
            return int.Parse($"{item.MemberId}{item.ShopId}");
        }

        public bool ContainsID(int id)
        {
            if (!_appointments.ContainsKey(id))
            {
                return MarketContext.GetInstance().Appointments.
                    Where(app => int.Parse($"{app.MemberId}{app.ShopId}") == id).FirstOrDefault() != null;
            }
            return true;
        }

        public bool ContainsValue(Appointment item)
        {
            int id = AppointmentUnicId(item);
            if (!_appointments.ContainsKey(id))
            {
                return MarketContext.GetInstance().Appointments.
                    Where(app => int.Parse($"{app.MemberId}{app.ShopId}") == id).FirstOrDefault() != null;
            }
            return true;
        }

        public void Delete(int memberId, int shopId)
        {
            int id = int.Parse($"{memberId}{shopId}");
            AppointmentDTO appDto = MarketContext.GetInstance().Appointments.
                    Where((a) => a.MemberId == memberId && a.ShopId == shopId).FirstOrDefault();
            if (!_appointments.ContainsKey(id))
            {
                if (appDto == null)
                {
                    throw new Exception("There is no appointment with this id");
                }
                MarketContext.GetInstance().Appointments.Remove(appDto);
                MarketContext.GetInstance().SaveChanges();
                return;
            }
            _appointments.Remove(id);
            MarketContext.GetInstance().Appointments.Remove(appDto);
            MarketContext.GetInstance().SaveChanges();
        }

        public List<Appointment> GetAll()
        {
            UploadAppointmentsFromContext();
            return _appointments.Values.ToList();
        }

        public void UploadAppointmentsFromContext()
        {
            List<AppointmentDTO> apps = MarketContext.GetInstance().Appointments.ToList();
            foreach (AppointmentDTO app in apps)
            {
                int id = AppointmentUnicId(app);
                if (!_appointments.ContainsKey(id))
                {
                    _appointments.TryAdd(id, new Appointment(app));
                    _appointments[id].InitializeComplexFeilds(app);
                }
            }
        }

        public Appointment GetById(int memberId, int shopId)
        {
            int id = int.Parse($"{memberId}{shopId}");
            if (_appointments.ContainsKey(id))
            {
                return _appointments[id];
            }
            else
            {
                MarketContext context = MarketContext.GetInstance();
                AppointmentDTO appDto = context.Appointments.Find(memberId, shopId);
                if (appDto == null)
                    throw new Exception("No appointment found");
                _appointments.TryAdd(id, new Appointment(appDto));
                _appointments[id].InitializeComplexFeilds(appDto);
                return _appointments[id];
            }

        }
        public void Update(Appointment item)
        {
            int id = int.Parse($"{item.Member.Id}{item.Shop.Id}");
            AppointmentDTO appDto = MarketContext.GetInstance().Appointments.
                    Where((a) => a.MemberId == item.Member.Id && a.ShopId == item.Shop.Id).FirstOrDefault();
            if (!_appointments.ContainsKey(id))
                Add(item);
            else
            {
                _appointments.Remove(id);
                _appointments.Add(id, item);
                MarketContext.GetInstance().Appointments.Update(appDto);
                MarketContext.GetInstance().SaveChanges();
            }
        }
        /// <summary>
        /// get all appointments of a given shopId
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns> return a dictionary of (userId, Appoinments) of all the appointents of a given shop
        public ConcurrentDictionary<int, Appointment> GetShopAppointments(int shopId)
        {
            UploadShopAppointments(shopId);
            ConcurrentDictionary<int, Appointment> shopAppointments = new ConcurrentDictionary<int, Appointment>();
            foreach (Appointment app in _appointments.Values)
            {
                if (app.Shop.Id == shopId) shopAppointments.TryAdd(app.Member.Id, app);
            }
            return shopAppointments;
        }

        private void UploadShopAppointments(int shopId)
        {
            List<AppointmentDTO> appDtos = MarketContext.GetInstance().Appointments
                .Where(app => app.ShopId == shopId).ToList();
            foreach (AppointmentDTO appDTO in appDtos)
            {
                int id = AppointmentUnicId(appDTO);
                if (!_appointments.ContainsKey(id))
                {
                    _appointments.TryAdd(id, new Appointment(appDTO));
                    _appointments[id].InitializeComplexFeilds(appDTO);
                }
            }
        }

        public void Clear()
        {
            _appointments.Clear();
        }

        public void ResetDomainData()
        {
            _appointmentRepo.Clear();
        }
    }
}