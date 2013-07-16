using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

using System.Drawing;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;

using System.Linq;
using Bs.Calendar.DataAccess;

namespace Bs.Calendar.Mvc.Services
{
    /// <summary>
    /// Class used by the RoomController class for edit room management
    /// </summary>
    public class RoomService
    {
        #region fields and properties
/*
        /// <summary>
        /// Allows to get edited Room instance
        /// </summary>
        public RoomEditVm Room
        {
            get;
            protected set;
        }
*/

        #endregion

        #region constructors

        public RoomService()
        {
//            Room = new RoomEditVm();
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
            {
                IList<Room> ilRooms = ruUnit.Room.Load(rRoom => rRoom.Id == iId).ToList();  /* 
                                                                                             * Select all rooms with matched id
                                                                                             * (actually, there should be 0 or 1 of them at all)
                                                                                             */

                if (ilRooms.Any())
                    revRoom = ilRooms.First();
            }

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
                IList<Room> ilRooms = ruUnit.Room.Load(rRoom => rRoom.Id == iId).ToList();  /* 
                                                                                             * Select all rooms with matched id
                                                                                             * (actually, there should be 0 or 1 of them at all)
                                                                                             */

                if (ilRooms.Any())
                {
                    RoomEditVm revRoom = ilRooms.First();
                    ruUnit.Room.Delete(revRoom);
                }
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
