using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using WebLedMatrix.Hubs;
using WebLedMatrix.Models;
using WebLedMatrix.Logic;
using System;

namespace WebLedMatrix
{
    public class MatrixManager
    {
        private HashSet<Matrix> matrices = new HashSet<Matrix>();
        public List<Matrix> Matrices => new List<Matrix>(matrices);

        private IHubContext<IUiManagerHub> Context { get; set; }

        public MatrixManager(IHubContext<IUiManagerHub> hubContext)
        {
            Context = hubContext;
        }

        public MatrixManager()
        {
            Context = GlobalHost.ConnectionManager.GetHubContext<IUiManagerHub>(typeof(UiManagerHub).Name);
        }

        public Matrix AddMatrix(string name)
        {
            var matrix = new Matrix() {Name = name};
            if (matrices.Any(x=>x.Name == name))
            {
                matrix = matrices.Single(x => x.Name == name);
            }
            else
            {
                matrices.Add(matrix);
            }
            UpdateMatrices();
            return matrix;
        }

        public void RemoveMatrix(string name)
        {
            matrices.RemoveWhere(x => x.Name == name);
            UpdateMatrices();
        }

        public void UpdateMatrices()
        {
            Context.Clients.All.unRegisterAllMatrices();
            Context.Clients.All.updateMatrices(matrices.ToArray());     
        }

        public void SendToAll(string username, string text)
        {
            foreach (var matrix in this.matrices)
            {
                matrix.AppendData(username,text);
            }
        }

        public void AppendData(string user, string clientName, string text)
        {
            var searchedMatrice = matrices.SingleOrDefault(x => x.Name.Equals(clientName));
            if (searchedMatrice == null)
            {
                throw new Exception($"Matrice named {clientName} does not exist at our cache. Please consider refreshing browser");
            }
            searchedMatrice.AppendData(user, text);
        }
    }
}