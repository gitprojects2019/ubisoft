using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using RouteAttribute = System.Web.Mvc.RouteAttribute;
using RoutePrefixAttribute = System.Web.Mvc.RoutePrefixAttribute;

namespace LeaderBoardService.Controllers
{
    [RoutePrefix("PlayerScores")]
    public class PlayerScores : ApiController
    {
        LeaderboardEntities db = new LeaderboardEntities();
        // GET api/<controller>
        [Route("GetByUsername")]
        public PlayerScore Get(String username)
        {
            PlayerScore playerScore = db.PlayerScores.Where(p => p.username == username).OrderByDescending(p => p.score).Take(1).FirstOrDefault();
            return playerScore;
        }


        [Route("GetByUserLogged")]
        public IHttpActionResult getUser()
        {
            if(System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {

                return RedirectToRoute("GetByUsername", new {username = System.Threading.Thread.CurrentPrincipal.Identity.Name });
            }
            return Ok();
        }

        // GET api/<controller>/5
        [Route("GetByTime")]
        public IHttpActionResult Get(String matchname, int timeframe)
        { 
            List<PlayerScore> listOfPlayers = db.PlayerScores.Where(x => x.matchname.Equals(matchname)).ToList();
            var result = listOfPlayers;
            switch (timeframe)
            {
                case (int)TIMEFRAME.daily:
                    result = listOfPlayers.Where(x => x.timestamp.Date.Equals(DateTime.Now.Date)).ToList();
                    break;

                case (int)TIMEFRAME.alltime:
                    
                    break;
                case (int)TIMEFRAME.weekly:
                    result = listOfPlayers.Where(x => x.timestamp.Date >= DateTime.Now.AddDays(-7).Date && x.timestamp.Date <= DateTime.Now.Date).ToList();
                    break;
                case (int)TIMEFRAME.last1Hr:
                    result = listOfPlayers.Where(x => x.timestamp >= DateTime.Now.AddHours(-1) && x.timestamp <= DateTime.Now).ToList();
                    break;
                case (int)TIMEFRAME.last5min:
                    result = listOfPlayers.Where(x => x.timestamp >= DateTime.Now.AddMinutes(-5) && x.timestamp <= DateTime.Now).ToList();
                    break;

            }
            return Ok(listOfPlayers);
        }

        [Route("GetAdjacentScores")]
        [HttpGet]
        public IHttpActionResult GetAdjacentScores(String matchname)
        {
            List<PlayerScore> listOfPlayers = db.PlayerScores.Where(x => x.matchname.Equals(matchname)).ToList();
            List<PlayerScore> result = new List<PlayerScore>();
            result.AddRange(listOfPlayers.OrderByDescending(x => x.score).Take(5).ToList());
            result.AddRange(listOfPlayers.OrderBy(x => x.score).Take(5).ToList());

            return Ok(result);
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody]PlayerScore value)
        {
            
            db.PlayerScores.Add(value);
            Task<int> rowsAdded = db.SaveChangesAsync();
             
            return Ok(rowsAdded);
        }
    }

    public enum TIMEFRAME
    {
        daily, 
        weekly, 
        alltime,
        last5min,
        last1Hr
    }
}