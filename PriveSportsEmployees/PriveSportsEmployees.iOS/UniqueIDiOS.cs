using System;
using Foundation;
using PriveSportsEmployees.Controls;
using Security;
using UIKit;

namespace PriveSportsEmployees.iOS
{
    public class UniqueIDiOS : IDevice
    {
        public string _id;

        public string GetIdentifier()
        {
            if (string.IsNullOrEmpty(_id))
            {

                var idRecord = new SecRecord(SecKind.GenericPassword);
                idRecord.Generic = NSData.FromString("myId");
                var matchingIdRecord = SecKeyChain.QueryAsRecord(idRecord, out var idResult);
                if (idResult == SecStatusCode.Success)
                {
                    _id = matchingIdRecord.ValueData.ToString();
                }
                else
                {
                    _id = System.Guid.NewGuid().ToString();

                    var newIdRecord = new SecRecord(SecKind.GenericPassword)
                    {
                        ValueData = NSData.FromString(_id),
                        Synchronizable = false,
                        Generic = NSData.FromString("myId")
                    };

                    var saveResponse = SecKeyChain.Add(newIdRecord);

                    if (saveResponse != SecStatusCode.Success && saveResponse != SecStatusCode.DuplicateItem)
                    {
                        _id = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                    }
                }
            }
            return _id;
            //   return UIDevice.CurrentDevice.IdentifierForVendor.AsString();

        }
    }
}
