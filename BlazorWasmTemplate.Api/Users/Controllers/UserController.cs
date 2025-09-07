using BlazorWasmTemplate.Application.Users.Dtos;
using BlazorWasmTemplate.Application.Users.UseCases.Query;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWasmTemplate.Api.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// ユーザークエリ ユースケース
        /// </summary>
        private readonly IUserQueryUseCase _userQueryUseCase;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userQueryUseCase"></param>
        public UserController(IUserQueryUseCase userQueryUseCase)
        {
            _userQueryUseCase = userQueryUseCase;
        }

        /// <summary>
        /// 全件取得
        /// </summary>
        /// <returns>ユーザーリスト</returns>
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await _userQueryUseCase.GetAllAsync();
            var dtoList = new List<UserDto>();
            foreach (var user in users)
            {
                var dto = new UserDto();
                dto.Id = user.Id;
                dto.Code = user.Code;
                dto.Name = user.Name;
                dto.IsActive = user.IsActive;
                dto.CreatedAt = user.CreatedAt;
                dto.UpdatedAt = user.UpdatedAt;
                dtoList.Add(dto);
            }

            return Ok(dtoList);
        }
    }
}