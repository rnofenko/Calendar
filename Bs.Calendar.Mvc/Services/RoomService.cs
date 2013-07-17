using System;

using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
    /// <summary>
    /// Class used by the RoomController class for edit room management
    /// </summary>
    public class RoomService
    {
        #region fields and properties

        private RoomEditVm _revRoom;

        /// <summary>
        /// Allows to get the edited room instance
        /// </summary>
        public RoomEditVm Room
        {
            get { return _revRoom; }
            protected set
            {
                if(value == null)
                    throw new ArgumentNullException("reference to the edited instance cannot be null");

                _revRoom = value;
            }
        }

        #endregion

        #region constructors

        public RoomService()
        {
            Room = new RoomEditVm();
        }

        #endregion

        #region methods

        /// <summary>
        /// Creates new instance if edited room view model
        /// </summary>
        public RoomEditVm CreateView()
        {
            return new RoomEditVm();
        }

        /// <summary>
        /// Saves edited room instance in the database
        /// </summary>
        /// <returns>true if save operation succeded, false - if any error occured</returns>
        public bool Save(RoomEditVm revRoom)
        {
            bool bResult = true;

#region validating edited room fields' data

            if (revRoom.NumberOfPlaces <= 0)
            {
                bResult = false;
                return bResult;
            }

#endregion

            using (var ruRepository = new RepoUnit())
            /* Connecting to the database and trying to save new record */
                ruRepository.Room.Save(revRoom);

            return bResult;
        }

        /// <summary>
        /// Searches for the room record by its id in the database
        /// </summary>
        public RoomEditVm Load(int iId)
        {
            RoomEditVm revRoom = null;

            using (var ruUnit = new RepoUnit())
                revRoom = ruUnit.Room.Get(iId);

            return revRoom;
        }

        /// <summary>
        /// Removes edit room instance from database. Instance is searched by it's ID
        /// </summary>
        public void Delete(int iId)
        {
            using (var ruUnit = new RepoUnit())
            /*
             * Use this instead of just this.Load() for achieving some kind of performance
             */
            {
                Room rRoom = ruUnit.Room.Get(iId);
                
                if(rRoom != null)
                    this.Delete(rRoom);
            }
        }

        /// <summary>
        /// Removes edit room instance from database.
        /// </summary>
        public void Delete(RoomEditVm revRoom)
        {
            using (var ruUnit = new RepoUnit())
                ruUnit.Room.Delete(revRoom);
        }

        #endregion
    }
}
