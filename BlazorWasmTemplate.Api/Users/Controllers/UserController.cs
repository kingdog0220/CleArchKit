using AutoMapper;
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
        /// Mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userQueryUseCase"></param>
        /// <param name="mapper"></param>
        public UserController(IUserQueryUseCase userQueryUseCase, IMapper mapper)
        {
            _userQueryUseCase = userQueryUseCase;
            _mapper = mapper;
        }

        /// <summary>
        /// 全件取得
        /// </summary>
        /// <returns>ユーザーリスト</returns>
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await _userQueryUseCase.GetAllAsync();
            var dtoList = _mapper.Map<List<UserDto>>(users);

            return Ok(dtoList);
        }
    }
}