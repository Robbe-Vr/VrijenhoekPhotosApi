using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VrijenhoekPhotos.Api.Filters;
using VrijenhoekPhotos.Core.Handlers;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Api.Controllers
{
    [EnableCors("VrijenhoekPhotosCorsPolicy")]
    [Authorized((int)Rights.CanRead)]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ProcessingController
    {
        private GroupsHandler _groups;
        public GroupsController(GroupsHandler groups, JsonSerializerSettings jsonSettings) : base(jsonSettings)
        {
            _groups = groups;
        }

        [HttpGet]
        [Authorized((int)Rights.CanRead)]
        public IActionResult All([FromQuery] int page = 1, [FromQuery] int rpp = 10, [FromQuery] string nameFilter = "", [FromQuery] Sorting sorting = Sorting.Newest_Oldest)
        {
            return Process(() =>
            {
                return _groups.All(page, rpp, nameFilter, sorting);
            });
        }

        [HttpGet("count")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Count([FromQuery] string nameFilter = "")
        {
            return Process(() =>
            {
                return _groups.Count(nameFilter);
            });
        }

        [HttpGet("seek")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Seek([FromQuery] string filter = null, [FromQuery] int page = 1, [FromQuery] int rpp = 10, [FromQuery] Sorting sorting = Sorting.Newest_Oldest)
        {
            return Process(() =>
            {
                return _groups.Seek(filter, page, rpp, sorting);
            });
        }

        [HttpGet("seek/count")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult SeekCount([FromQuery] string filter = null)
        {
            return Process(() =>
            {
                return _groups.GetSeekCount(filter);
            });
        }

        [HttpGet("joined")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult AllJoined([FromQuery] int page = 1, [FromQuery] int rpp = 10, [FromQuery] string nameFilter = "", [FromQuery] Sorting sorting = Sorting.Newest_Oldest)
        {
            return Process(() =>
            {
                return _groups.AllJoined(page, rpp, nameFilter, sorting);
            });
        }

        [HttpGet("joined/count")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult JoinedCount([FromQuery] string nameFilter = "")
        {
            return Process(() =>
            {
                return _groups.JoinedCount(nameFilter);
            });
        }

        [HttpGet("owned")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult AllOwned([FromQuery] int page = 1, [FromQuery] int rpp = 10, [FromQuery] string nameFilter = "", [FromQuery] Sorting sorting = Sorting.Newest_Oldest)
        {
            return Process(() =>
            {
                return _groups.AllOwned(page, rpp, nameFilter, sorting);
            });
        }

        [HttpGet("owned/count")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult OwnedCount([FromQuery] string nameFilter = "")
        {
            return Process(() =>
            {
                return _groups.OwnedCount(nameFilter);
            });
        }

        [HttpGet("{groupId}/albums")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Albums(int groupId, [FromQuery] int page, [FromQuery] int rpp = 10, [FromQuery] string nameFilter = "", [FromQuery] Sorting sorting = Sorting.Newest_Oldest)
        {
            return Process(() =>
            {
                return _groups.AlbumsInGroup(groupId, page, rpp, nameFilter, sorting);
            });
        }

        [HttpGet("{groupId}/albums/count")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult AlbumsCount(int groupId, [FromQuery] string nameFilter = "")
        {
            return Process(() =>
            {
                return _groups.AlbumsInGroupCount(groupId, nameFilter);
            });
        }

        [HttpGet("{groupId}/users")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Users(int groupId, [FromQuery] int page, [FromQuery] int rpp = 10, [FromQuery] string nameFilter = "", [FromQuery] Sorting sorting = Sorting.Newest_Oldest)
        {
            return Process(() =>
            {
                return _groups.UsersInGroup(groupId, page, rpp, nameFilter, sorting);
            });
        }

        [HttpGet("{groupId}/users/count")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult UsersCount(int groupId, [FromQuery] string nameFilter = "")
        {
            return Process(() =>
            {
                return _groups.UsersInGroupCount(groupId, nameFilter);
            });
        }

        [HttpGet("{id}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult GetById(int id)
        {
            return Process(() =>
            {
                return _groups.ById(id);
            });
        }

        [HttpGet("byname/{name}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult GetByName(string name)
        {
            return Process(() =>
            {
                return _groups.ByName(name);
            });
        }

        [HttpPost("{groupId}/add/{albumId}")]
        [Authorized((int)Rights.CanUpdate)]
        public IActionResult AddAlbumToGroup(int groupId, int albumId)
        {
            return Process(() =>
            {
                return _groups.AddAlbum(groupId, albumId);
            });
        }

        [HttpPost("request-join/{groupId}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult RequestJoin(int groupId)
        {
            return Process(() =>
            {
                return _groups.RequestJoin(groupId);
            });
        }

        [HttpPost("{groupId}/accept-request/{userId}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult AcceptJoinRequest(int groupId, string userId)
        {
            return Process(() =>
            {
                return _groups.AcceptRequest(groupId, userId);
            });
        }

        [HttpPost("{groupId}/decline-request/{userId}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult DeclineJoinRequest(int groupId, string userId)
        {
            return Process(() =>
            {
                return _groups.DeclineRequest(groupId, userId);
            });
        }

        [HttpPost("{groupId}/invite/{userId}")]
        [Authorized((int)Rights.CanCreate)]
        public IActionResult InviteJoin(int groupId, string userId)
        {
            return Process(() =>
            {
                return _groups.Invite(groupId, userId);
            });
        }

        [HttpPost("{groupId}/accept-invite")]
        [Authorized((int)Rights.CanCreate)]
        public IActionResult AcceptInvite(int groupId)
        {
            return Process(() =>
            {
                return _groups.AcceptInvite(groupId);
            });
        }

        [HttpPost("{groupId}/decline-invite")]
        [Authorized((int)Rights.CanCreate)]
        public IActionResult DeclineInvite(int groupId)
        {
            return Process(() =>
            {
                return _groups.DeclineInvite(groupId);
            });
        }

        [HttpPost("{groupId}/remove/{albumId}")]
        [Authorized((int)Rights.CanCreate)]
        public IActionResult RemoveAlbumFromGroup(int groupId, int albumId)
        {
            return Process(() =>
            {
                return _groups.RemoveAlbum(groupId, albumId);
            });
        }

        [HttpPost("{groupId}/removeuser/{userId}")]
        [Authorized((int)Rights.CanCreate)]
        public IActionResult RemoveUserFromGroup(int groupId, string userId)
        {
            return Process(() =>
            {
                return _groups.RemoveUser(groupId, userId);
            });
        }

        [HttpPost("{action}/{name}")]
        [Authorized((int)Rights.CanCreate)]
        public IActionResult Create(string name)
        {
            return Process(() =>
            {
                return _groups.Create(name);
            });
        }

        [HttpPost("{action}")]
        [Authorized((int)Rights.CanUpdate)]
        public IActionResult Update(GroupDTO group)
        {
            return Process(() =>
            {
                return _groups.Update(group);
            });
        }

        [HttpPost("{action}")]
        [Authorized((int)Rights.CanRead)]
        public IActionResult Remove(GroupDTO group)
        {
            return Process(() =>
            {
                return _groups.Remove(group);
            });
        }
    }
}
