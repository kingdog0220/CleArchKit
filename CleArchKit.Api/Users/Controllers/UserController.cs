using CleArchKit.Application.Users.Dtos;
using CleArchKit.Application.Users.UseCases.Query;
using Microsoft.AspNetCore.Mvc;

namespace CleArchKit.Api.Users.Controllers
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
            var dtoList = users.Select(UserDto.From).ToList();

            return Ok(dtoList);
        }
    }
}