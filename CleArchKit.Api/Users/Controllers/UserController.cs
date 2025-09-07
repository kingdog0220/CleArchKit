using CleArchKit.Application.Expands;
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
        /// ユースケースで共通して行う処理を拡張したラッパー
        /// </summary>
        private readonly IUseCaseExecutor _useCaseExecutor;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userQueryUseCase"></param>
        /// <param name="userCommandUseCase"></param>
        /// <param name="useCaseExecutor"></param>
        public UserController(IUserQueryUseCase userQueryUseCase, IUserCommandUseCase userCommandUseCase, IUseCaseExecutor useCaseExecutor)
        {
            _userQueryUseCase = userQueryUseCase;
            _userCommandUseCase = userCommandUseCase;
            _useCaseExecutor = useCaseExecutor;
        }

        /// <summary>
        /// 全件取得
        /// </summary>
        /// <returns>ユーザーリスト</returns>
        [HttpGet("all")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAll()
        {
            var users = await _userQueryUseCase.GetAllAsync();
            var dtoList = users.Select(UserResponseDto.From).ToList();

            return Ok(dtoList);
        }

        /// <summary>
        /// IDによるユーザー取得
        /// </summary>
        /// <param name="id">物理ID</param>
        /// <returns>ユーザー</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetByIdAsync(Guid id)
        {
            var user = await _userQueryUseCase.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var dto = UserResponseDto.From(user);
            return Ok(dto);
        }

        /// <summary>
        /// ユーザー登録
        /// </summary>
        /// <param name="createUserDto"></param>
        [HttpPost("create")]
        public async Task<ActionResult> CreateAsync(CreateUserDto createUserDto)
        {
            try
            {
                await _useCaseExecutor.ExecuteAsync(() => _userCommandUseCase.CreateAsync(createUserDto));
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
        /// <param name="updateUserDto"></param>
        [HttpPut("update")]
        public async Task<ActionResult> UpdateAsync(UpdateUserDto updateUserDto)
        {
            try
            {
                await _useCaseExecutor.ExecuteAsync(() => _userCommandUseCase.UpdateAsync(updateUserDto));
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
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _useCaseExecutor.ExecuteAsync(() => _userCommandUseCase.DeleteAsync(id));
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