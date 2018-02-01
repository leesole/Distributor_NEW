using Distributor.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Distributor.Helpers
{
    public static class GeneralHelpers
    {
        public static Guid StringToGuid(string stringGuid)
        {
            Guid guidId = Guid.Empty;
            Guid.TryParse(stringGuid, out guidId);

            return guidId;
        }

        public static void GetCallingDetailsFromRequest(HttpRequestBase request, string controllerInitialValue, string actionInitialValue, out string controller, out string action)
        {
            controller = controllerInitialValue;
            action = actionInitialValue;

            try
            {
                string[] callingUrlSegments = request.UrlReferrer.Segments.Select(x => x.TrimEnd('/')).ToArray();
                controller = callingUrlSegments[callingUrlSegments.Count() - 2];
                action = callingUrlSegments[callingUrlSegments.Count() - 1];
                if (request.UrlReferrer.Query.Length > 0)
                    action += request.UrlReferrer.Query;
            }
            catch { }
        }


    }

    public static class SearchHelpers
    {
        public static List<AvailableListing> FilterAvailableListingsByDistance(ApplicationDbContext db, List<AvailableListing> currentList, Guid organisationId, int maxDistanceFilter)
        {
            //get this organisation post code to match against those of the listed organisations
            Organisation thisOrganisation = OrganisationHelpers.GetOrganisation(db, organisationId);

            //hold list of organisationIds already checked - set to true if within range
            Dictionary<Guid, bool> listingOrgIds = new Dictionary<Guid, bool>();

            //hold new list from old
            List<AvailableListing> newList = new List<AvailableListing>();

            foreach (AvailableListing item in currentList)
            {
                //if we have checked this org before then just add or not depending on flag
                if (listingOrgIds.ContainsKey(item.ListingOriginatorOrganisationId))
                {
                    if (listingOrgIds[item.ListingOriginatorOrganisationId])
                        newList.Add(item);
                }
                else  //add the org to the dictionary with the flag set and add to new list if within range
                {
                    int distanceValue = DistanceHelpers.GetDistance(thisOrganisation.AddressPostcode, item.ListingOrganisationPostcode);
                    if (distanceValue <= maxDistanceFilter)
                    {
                        listingOrgIds.Add(item.ListingOriginatorOrganisationId, true);
                        newList.Add(item);
                    }
                    else
                        listingOrgIds.Add(item.ListingOriginatorOrganisationId, false);
                }
            }

            return newList;
        }
    }

    public static class DistanceHelpers
    {
        public static int GetDistance(string origin, string destination)
        {
            double CalcMetersToMiles = 0.00062137;

            System.Threading.Thread.Sleep(1000);
            int distance = 0;
            //string from = origin.Text;
            //string to = destination.Text;
            string url = "http://maps.googleapis.com/maps/api/directions/json?origin=" + origin + "&destination=" + destination + "&sensor=false";
            string requesturl = url;
            //string requesturl = @"http://maps.googleapis.com/maps/api/directions/json?origin=" + from + "&alternatives=false&units=imperial&destination=" + to + "&sensor=false";
            string content = FileGetContents(requesturl);
            JObject o = JObject.Parse(content);
            try
            {
                distance = (int)o.SelectToken("routes[0].legs[0].distance.value");
                distance = Convert.ToInt32(Math.Floor((double)distance * CalcMetersToMiles));
                return distance;
            }
            catch
            {
                return distance;
            }
            //ResultingDistance.Text = distance;
        }

        private static string FileGetContents(string fileName)
        {
            string sContents = string.Empty;
            string me = string.Empty;
            try
            {
                if (fileName.ToLower().IndexOf("http:") > -1)
                {
                    System.Net.WebClient wc = new System.Net.WebClient();
                    byte[] response = wc.DownloadData(fileName);
                    sContents = System.Text.Encoding.ASCII.GetString(response);

                }
                else
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                    sContents = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch { sContents = "unable to connect to server "; }
            return sContents;
        }
    }
}