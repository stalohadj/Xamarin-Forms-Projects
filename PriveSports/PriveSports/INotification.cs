using System;
using System.Collections.Generic;
using System.Text;

namespace PriveSports
{
    public interface INotification
    {
        void CreateNotification(String title, String message);
    }
}

