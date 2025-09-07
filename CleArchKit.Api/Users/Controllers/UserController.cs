using CleArchKit.Application.Users.Dtos;
using CleArchKit.Application.Users.UseCases.Command;
using CleArchKit.Application.Users.UseCases.Query;
using Microsoft.AspNetCore.Mvc;

namespace CleArchKit.Api.Users.Controllers
{
    /// <summary>
    /// ユーザー コントローラー
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// ユーザークエリ ユースケース
        /// </summary>
        private readonly IUserQueryUseCase _userQueryUseCase;

        /// <summary>
        /// ユーザーコマンド ユースケース
        /// </summary>
        private readonly IUserCommandUseCase _userCommandUseCase;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userQueryUseCase"></param>
        /// <param name="userCommandUseCase"></param>
        public UserController(IUserQueryUseCase userQueryUseCase, IUserCommandUseCase userCommandUseCase)
        {
            _userQueryUseCase = userQueryUseCase;
            _userCommandUseCase = userCommandUseCase;
        }

        /// <summary>
        /// 全件取得
        /// </summary>
        /// <returns>ユーザーリスト</returns>
        [HttpGet("all")]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await _userQueryUseCase.GetAllAsync();
            var dtoList = users.Select(UserDto.From).ToList();

            return Ok(dtoList);
        }

        /// <summary>
        /// IDによるユーザー取得
        /// </summary>
        /// <param name="id">物理ID</param>
        /// <returns>ユーザー</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id)
        {
            var user = await _userQueryUseCase.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var dto = UserDto.From(user);
            return Ok(dto);
        }

        /// <summary>
        /// ユーザー登録
        /// </summary>
        /// <param name="userDto"></param>
        [HttpPost("create")]
        public async Task<ActionResult> CreateAsync(UserDto userDto)
        {
            try
            {
                await _userCommandUseCase.CreateAsync(userDto);
                return Ok();
            }
            catch (Exception ex)
            {
                // 例外処理はサンプルです。
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// ユーザー更新
        /// </summary>
        /// <param name="userDto"></param>
        [HttpPut("update")]
        public async Task<ActionResult> UpdateAsync(UserDto userDto)
        {
            try
            {
                await _userCommandUseCase.UpdateAsync(userDto);
                return Ok();
            }
            catch (Exception ex)
            {
                // 例外処理はサンプルです。
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// ユーザー削除
        /// </summary>
        /// <param name="userDto"></param>
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteAsync(UserDto userDto)
        {
            try
            {
                await _userCommandUseCase.DeleteAsync(userDto);
                return Ok();
            }
            catch (Exception ex)
            {
                // 例外処理はサンプルです。
                return BadRequest(ex.Message);
            }
        }
    }
}