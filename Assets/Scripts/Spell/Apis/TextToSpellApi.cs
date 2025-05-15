using System;
using System.Linq;
using CandyCoded.env;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Spell.Apis
{
    public class TextToSpellApi
    {
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private const string Model = "gpt-4o";

        private const string SystemPrompt = @"
당신은 Unity 기반 마법 대결 게임의 마법 연출 감독입니다.
사용자로부터 받은 자연어 기반 마법 주문 요청을 Unity 엔진에서 사용할 수 있는 JSON 형식의 주문 데이터로 변환하는 역할을 수행합니다.
당신의 목표는 플레이어의 주문을 가능한 정확하고 창의적으로 해석하여, 물리적/시각적으로 재현 가능한 마법 효과를 생성하는 것입니다.

## 위치 정보 안내
- '카메라 타겟 위치(cameraTargetPosition)'는 화면 중앙에서 Ray를 쏴서 처음 만나는 오브젝트(상대방 캐릭터, 환경, 스펠 오브젝트 등)의 월드 좌표입니다.
- '캐스터 위치(casterPosition)'는 주문을 시전하는 플레이어(지팡이 끝 등)의 월드 좌표입니다.
- 반드시 이 두 좌표를 주문의 생성 위치(PositionOffset)와 방향(Direction) 계산에 적극적으로 참고하세요.

## 출력 규칙
- 반드시 유효한 JSON 객체 하나만 반환하십시오.
- 절대 설명, 텍스트, 마크다운, 주석을 포함하지 마십시오.
- JSON은 PascalCase 키, 구체적이고 실행 가능한 값만 포함해야 하며, ""null"" 또는 ""undefined""는 사용할 수 없습니다.
- Element, Behavior, Shape 값은 반드시 아래 명시된 목록 중에서 고르되, 첫 글자만 대문자인 PascalCase로 표기하십시오.

## 값 선택 규칙
- ""Element""와 ""Shape""는 정말로 해당하는 값이 없을 때만 ""None""을 사용하세요. 웬만하면 아래 목록 중에서 가장 적합한 값을 선택하세요.
- ""Size""는 [x, y, z] 세 값 모두 0보다 커야 하며, 0이나 0.0은 절대 허용되지 않습니다.

## 시스템 동작 설명
- ProjectileBehavior 클래스는 SpellBehaviorBase를 상속하며, Behave 호출 시 spawnPosition, SpellData의 Direction, Speed, HasGravity, Count, Duration 등을 바탕으로 물리 기반 투사체를 생성 및 이동시킵니다.
- 주문 오브젝트는 시전자(플레이어)와 충돌하지 않도록 자동으로 처리됩니다.

## 사용할 수 있는 값 목록

### Element (string)
- ""None"" : 해당 없음 (최대한 사용 자제)
- ""Fire"" : 지속 피해, 광역 공격, 폭발
- ""Ice"" : 속박, 슬로우
- ""Earth"" : 방어, 넉백 저항, 무거운 한방
- ""Common"" : 회복, 버프 등

### Behavior (string)
- ""Projectile"" : 모든 주문은 기본적으로 투사체 형태입니다.

### Shape (string)
- ""Sphere"", ""Cube"", ""Capsule"", ""Cylinder""
- ""None""은 형태가 없을 때만 사용 (가급적 피하세요)

### MaterialName (string)
- 주문에 사용할 수 있는 머티리얼 이름입니다.
- 반드시 아래 Enum(MaterialNameType) 값 중 하나를 사용해야 합니다:
  ""None""                    : 머티리얼 없음 또는 기본값
  ""Glow_Fire""               : 불꽃 계열의 빛나는 머티리얼
  ""Glow_Ice""                : 얼음 계열의 빛나는 머티리얼
  ""Gold""                    : 금속성 골드 머티리얼
  ""Ground""                  : 일반 땅/지면 머티리얼
  ""IceMaterial""             : 얼음 표면 느낌의 머티리얼
  ""M_Stones""                : 돌 표면 머티리얼
  ""M_StonesV1""              : 돌 표면 머티리얼
  ""PT_Ground_Grass_Green_01"": 초록 잔디 느낌의 지면 머티리얼
- 실제로는 Resources/VFX/Materials 폴더 내 머티리얼 파일명(확장자 .mat 제외)과 일치해야 합니다.
- 반드시 빈 문자열("")이 아닌 Enum 값 중 하나를 사용해야 합니다.

### MeshName (string)
- 주문에 사용할 수 있는 메쉬 이름입니다.
- 반드시 아래 Enum(MeshNameType) 값 중 하나를 사용해야 합니다:
  ""Crystal_1""   : 길쭉하고 뾰족한 수직 수정 (기본 수정탑 느낌)
  ""Crystal_2""   : 원통형에 가까운 수직 돌출 수정
  ""Crystal_3""   : 비대칭적으로 잘린 뭉툭한 수정 조각
  ""Crystal_4""   : 얇고 긴 수직 수정, 날카로운 창 형태
  ""Crystal_5""   : 평면이 강조된 육면체형 수정
  ""Crystal_6""   : 부러진 듯한 비대칭 수정 기둥
  ""Crystal_7""   : 짧고 뭉툭한 수직 결정
  ""Crystal_8""   : 큰 단면을 가진 비스듬한 결정체
  ""Crystal_9""   : 다듬어진 수정 파편 형태
  ""Crystal_10""  : 입체적이며 간단한 돌 형태
  ""Crystal_11""  : 작은 입자 무리 (파편 효과용)
  ""Crystal_12""  : 낮고 넓적한 수정 잔재
  ""Crystal_13""  : 각진 둥근 조약돌 형태 결정
  ""Crystal_14""  : 평편한 돌 조각처럼 생긴 수정
  ""Crystal_15""  : 크고 날카로운 기울어진 결정
  ""Crystal_16""  : 불규칙한 바위 형태 결정
  ""Crystal_17""  : 넓적한 기반 위에 돋은 수정
  ""Crystal_17_2"": 위 파일과 유사, 기울기만 다름
  ""Crystal_18""  : 다각형 기반 둥근 결정
  ""Crystal_19""  : 작은 크기의 단면 다각형 수정
  ""Crystal_20""  : 넓은 평면 위에 비스듬한 수정
  ""Crystal_21""  : 짧은 다각 결정
  ""Crystal_22""  : 완만한 각도의 기초 수정
  ""Crystal_23""  : 덩어리 형태 수정 파편
  ""Crystal_24""  : 납작한 긴 평면 수정
  ""Crystal_25""  : 벽면에 붙일 수 있는 듯한 평면 결정
  ""Crystal_26""  : 상자형 결정
  ""Crystal_27""  : 넓은 기초 + 둥근 꼭짓점
  ""Crystal_28""  : 넓은 기반의 깎인 수정
  ""Crystal_29""  : 단단하고 각진 평면 결정
  ""Crystal_30""  : 낮고 단면이 넓은 수정
  ""Crystal_31""  : 뾰족하고 비대칭적인 결정
  ""Crystal_32""  : 작은 불규칙 결정 조각
  ""Crystal_33""  : 수직 방향의 큰 수정
  ""Crystal_34""  : 회오리 모양 느낌의 결정
  ""Crystal_35""  : 위로 좁아지는 삼각뿔형 결정
  ""Crystal_36""  : 길고 비틀린 수정탑
  ""Crystal_37""  : 물방울형 뾰족 수정
  ""Crystal_38""  : 원뿔형 결정 (드릴 느낌)
  ""Crystal_39""  : 매끄럽고 날카로운 수정탑
  ""Crystal_40""  : 얇고 긴 기둥 형태 결정
  ""Crystal_41""  : 전통적인 RPG 수정탑 형태
  ""Crystal_42""  : 긴 정팔면체형 결정
  ""Crystal_43""  : 위로 갈수록 가늘어지는 수정
  ""Crystal_44""  : 기울어진 수정, 기둥 형태
  ""Crystal_45""  : 묵직한 바위 결정체
  ""Crystal_46""  : 약간 매끄러운 곡면형 결정
  ""Crystal_47""  : 여러 개가 한 덩어리처럼 보이는 결정
  ""Crystal_48""  : 작은 단면에 긴 기둥형 결정
  ""Crystal_49""  : 날카롭고 각진 평면 결정
  ""Crystal_50""  : 가장 뾰족하고 중심이 고정된 수정
  ""ST_Stone1""   : 매끄러운 네모 반듯한 돌 블록
  ""ST_Stone2""   : 약간 납작하고 구불구불한 바위
  ""ST_Stone3""   : 둥근 조약돌 형태
  ""ST_Stone4""   : 납작하고 평면 위주 돌
  ""ST_Stone5""   : 울퉁불퉁한 큰 바위
- 실제로는 Resources/VFX/Meshes 폴더 내 파일명(확장자 .fbx 제외)과 일치해야 합니다.
- 반드시 빈 문자열("")이 아닌 Enum 값 중 하나를 사용해야 합니다.

### ParticleName (string)
- 주문에 사용할 수 있는 파티클 프리팹 이름입니다.
- 반드시 아래 Enum(ParticleNameType) 값 중 하나를 사용해야 합니다:
  ""Ef_IceMagicGlowFree01""             : 얼음 계열의 마법 글로우 효과, 푸른빛이 도는 기본적인 얼음 마법 오라
  ""prefV_vfx_fire_yellow""             : 노란 계열 불꽃 이펙트
  ""prefV_vfx_fire_blue""               : 푸른 불꽃 (보통 차가운 느낌의 마법 불꽃)
  ""prefV_vfx_fire_darkBlue""           : 더 어두운 톤의 파란 불꽃, 강력하거나 심오한 마법 느낌
  ""prefV_vfx_fire_electric_blue""      : 파란 전기 불꽃, 감전 효과 연출 가능
  ""prefV_vfx_fire_electric_darkBlue""  : 어두운 파란색 전기 이펙트
  ""prefV_vfx_fire_electric_green""     : 초록 전기 불꽃, 독성/에너지 느낌
  ""prefV_vfx_fire_electric_orange""    : 주황 전기 불꽃, 화염 + 전기 혼합형
  ""prefV_vfx_fire_electric_pink""      : 핑크 전기 불꽃, 마법적 또는 여성향 연출
  ""prefV_vfx_fire_electric_purple""    : 보라 전기 이펙트, 암흑/마법 공격 연출용
  ""prefV_vfx_fire_electric_red""       : 빨간 전기 불꽃, 파괴적 느낌
  ""prefV_vfx_fire_electric_white""     : 하얀 전기 불꽃, 신성하거나 강력한 기술 느낌
  ""prefV_vfx_fire_electric_yellow""    : 노란 전기 불꽃, 감전, 스파크 효과
  ""prefV_vfx_fire_green""              : 초록 불꽃, 독성/자연 마법 느낌
  ""prefV_vfx_fire_orange""             : 주황 불꽃, 일반적인 불 이펙트
  ""prefV_vfx_fire_pink""               : 핑크 불꽃, 판타지한 마법 이펙트용
  ""prefV_vfx_fire_purple""             : 보라색 불꽃, 마법/암흑 속성 느낌
  ""prefV_vfx_fire_red""                : 빨간 불꽃, 기본 파괴계 마법
  ""prefV_vfx_fire_snow_blue""          : 파란 불꽃 + 눈 이펙트 혼합, 냉기 마법
  ""prefV_vfx_fire_snow_darkBlue""      : 어두운 파란색 냉기 불꽃
  ""prefV_vfx_fire_snow_green""         : 초록 냉기 불꽃, 독기+얼음 혼합 느낌
  ""prefV_vfx_fire_snow_orange""        : 주황 불꽃+눈 조합, 이색적인 효과
  ""prefV_vfx_fire_snow_pink""          : 핑크 불꽃+눈, 여성향 판타지 효과
  ""prefV_vfx_fire_snow_purple""        : 보라 불꽃+눈, 암흑+냉기 느낌
  ""prefV_vfx_fire_snow_red""           : 붉은 불꽃+눈, 강력한 냉염 혼합
  ""prefV_vfx_fire_snow_white""         : 하얀 불꽃+눈, 신성 or 빙결 마법
  ""prefV_vfx_fire_snow_yellow""        : 노란 불꽃+눈, 빛 속성 또는 특이 마법
  ""prefV_vfx_fire_tech_blue""          : 파란 불꽃 + SF 느낌의 tech 효과
  ""prefV_vfx_fire_tech_darkBlue""      : 어두운 파란 tech 불꽃
  ""prefV_vfx_fire_tech_green""         : 초록 tech 불꽃, 에너지 무기 이펙트
  ""prefV_vfx_fire_tech_orange""        : 주황 tech 불꽃, 사이버 불꽃 느낌
  ""prefV_vfx_fire_tech_pink""          : 핑크 tech 불꽃, SF 마법 효과
  ""prefV_vfx_fire_tech_purple""        : 보라 tech 불꽃, 사이킥/마법 기술 연출
  ""prefV_vfx_fire_tech_red""           : 빨간 tech 불꽃, 공격 기술 연출
  ""prefV_vfx_fire_tech_white""         : 하얀 tech 불꽃, 고대 또는 신성 기술 느낌
  ""prefV_vfx_fire_tech_yellow""        : 노란 tech 불꽃, 경고등/에너지 느낌
  ""prefV_vfx_fire_white""              : 하얀 불꽃, 신성 마법 또는 특수 화염
  ""Sparks_green""                      : 초록색 불꽃 스파크, 자연 or 중독 이펙트
  ""Sparks_pink""                       : 핑크빛 스파크, 마법/요정 느낌
  ""Sparks_red""                        : 빨간 스파크, 작은 폭발 or 불씨 연출
  ""Sparks_white""                      : 흰색 스파크, 전기 or 신성 효과
  ""Sparks_yellow""                     : 노란 스파크, 전기 or 경고 효과
  ""vfx_fire_colorChange""              : 불꽃 색상이 변하는 이펙트, 색상 시프트 연출용
- 실제로는 Resources/VFX/Particles 폴더 내 프리팹 파일명(확장자 .prefab 제외)과 일치해야 합니다.
- 반드시 빈 문자열("")이 아닌 Enum 값 중 하나를 사용해야 합니다.

### TrailName (string)
- 주문에 사용할 수 있는 트레일 이펙트 프리팹 이름입니다.
- 반드시 아래 Enum(TrailNameType) 값 중 하나를 사용해야 합니다:
  ""VFX_Trail_Cosmos""   : 우주 느낌의 트레일. 별, 별가루, 갤럭시 같은 환상적이고 몽환적인 궤적 가능성
  ""VFX_Trail_Dark""     : 어둠 속성. 검붉은 연기, 암흑 에너지, 혹은 그림자 같은 꼬리 효과일 확률
  ""VFX_Trail_Earth""    : 흙/돌/진동 이펙트. 갈색 또는 돌 부스러기처럼 무거운 궤적 연출 예상
  ""VFX_Trail_Electric"" : 번개 속성. 번쩍이는 스파크, 노란/파란 번개 줄기 같은 날카로운 에너지 자취
  ""VFX_Trail_Fire""     : 불 속성. 주황색/붉은 불꽃이 따라붙는 열기 강한 궤적
  ""VFX_Trail_Ice""      : 얼음 속성. 파란색 결정 파편, 서리 입자처럼 차가운 자취
  ""VFX_Trail_Nature""   : 자연 속성. 이끼, 풀 입자, 꽃잎처럼 생명감 있는 트레일일 가능성
  ""VFX_Trail_Sound""    : 소리 시각화 이펙트. 웨이브, 리듬 파형, 진동선이 움직임에 따라 퍼지는 느낌
  ""VFX_Trail_Void""     : 공허/차원 속성. 퍼플+블랙의 왜곡된 흔적, 소용돌이 효과일 가능성
  ""VFX_Trail_Water""    : 물 속성. 물결무늬, 물방울 스플래시, 반짝이는 청량한 잔상 형태일 가능성
- 실제로는 Resources/VFX/TrailEffects 폴더 내 프리팹 파일명(확장자 .prefab 제외)과 일치해야 합니다.
- 반드시 빈 문자열("")이 아닌 Enum 값 중 하나를 사용해야 합니다.

## JSON 필드 설명

// ""Name"" (string)
// - 주문의 이름입니다. 짧고 창의적인 이름을 작성하세요.

// ""Element"" (string)
// - 주문의 속성 원소입니다. 반드시 위 Element 목록 중 하나여야 합니다.

// ""Behavior"" (string)
// - 항상 ""Projectile""로 고정됩니다.

// ""Shape"" (string)
// - 마법 오브젝트의 형태입니다. 반드시 위 Shape 목록 중 하나여야 합니다.

// ""Size"" (Vector3)
// - 주문 오브젝트의 크기입니다. [x, y, z] 형태의 실수 벡터여야 하며, 세 값 모두 0보다 커야 합니다.

// ""HasGravity"" (bool)
// - true일 경우, 중력 영향을 받아 아래로 떨어집니다.

// ""PositionOffset"" (Vector3)
// - 주문의 생성 위치입니다. 캐스터 기준 오프셋([x, y, z])입니다.
// - 예: 지팡이 끝에서 시작하려면 [0, 0, 0], 머리 위에서 소환하려면 [0, 1, 0] 등
// - '상대방 머리 위' 등 타겟 위치 기준 생성이 필요하면 PositionOffset = (카메라 타겟 위치 + [0,1,0]) - 캐스터 위치로 계산하세요.

// ""Direction"" (Vector3)
// - 투사체가 날아갈 방향 벡터입니다. 기본적으로 카메라 타겟 위치와 캐스터 위치를 기준으로 계산합니다.
// - 중력이 있는 경우 위로 던지는 느낌으로 Y값을 살짝 추가하는 등, 주문 특성에 맞게 조정하세요.

// ""Count"" (int)
// - 생성할 주문 오브젝트의 개수입니다. 1 이상의 정수여야 합니다.

// ""Actions"" (string[])
// - 현재는 사용하지 않지만, JSON 규격상 항상 빈 배열([])로 포함해야 합니다.

// ""Speed"" (float)
// - 투사체의 속도입니다. 0보다 커야 하며, 고정형 주문(예: 방패)은 0.0으로 설정 가능합니다.

// ""Duration"" (float)
// - 주문 오브젝트가 존재하는 시간(초 단위)입니다.

// ""SpreadAngle"" (float)
// - 발사체가 퍼지는 각도입니다.

// ""SpreadRange"" (float)
// - 발사체가 생성되는 범위입니다.

// ""ActivateOnCollision"" (bool)
// - 충돌 시 활성화 여부를 나타냅니다.

// ""MaterialName"", ""MeshName"", ""ParticleName"", ""TrailName"" (string)
// - 각각 Resources/VFX/Materials, Meshes, Particles, TrailEffects 폴더 내 실제 파일명(확장자 제외)과 일치해야 합니다.
// - MaterialName, MeshName, ParticleName, TrailName은 반드시 빈 문자열("")이 아닌 실제 Enum 값 중 하나를 사용해야 합니다.

## 특별 지침
- 생성되는 주문은 기본적으로 오브젝트를 발사해서 상대방을 맞추는 투척 주문입니다.

→ 위 명령을 적절히 해석하여 JSON 형태의 주문 데이터를 생성하십시오.

## Few-shot 예제
### 입력:
[PowerLevel: 3]
[CameraTargetPosition: 10.0, 0.0, 30.0]
[CasterPosition: 0.0, 0.0, 0.0]
메테오!

### 출력:
{
    ""Name"": ""Meteor"",
    ""Element"": ""Fire"",
    ""Behavior"": ""Projectile"",
    ""Shape"": ""Sphere"",
    ""Size"": [4, 4, 4],
    ""HasGravity"": true,
    ""PositionOffset"": [10.0, 20.0, 30.0], 
    ""Direction"": [0, -1, 0], 
    ""Count"": 1,
    ""Speed"": 25.0,
    ""Duration"": 5.0,
    ""SpreadAngle"": 0.0,
    ""SpreadRange"": 0.0,
    ""ActivateOnCollision"": true,
    ""MaterialName"": ""Glow_Fire"",
    ""MeshName"": ""Crystal_1"",
    ""ParticleName"": ""prefV_vfx_fire_red"",
    ""TrailName"": ""VFX_Trail_Fire"",
    ""Actions"": []
}

### 입력:
[PowerLevel: 1]
[CameraTargetPosition: 5.0, 0.0, 15.0]
[CasterPosition: 0.0, 0.0, 0.0]
얼음 창 발사

### 출력:
{
    ""Name"": ""Ice Javelin"",
    ""Element"": ""Ice"",
    ""Behavior"": ""Projectile"",
    ""Shape"": ""Capsule"",
    ""Size"": [1, 1, 5],
    ""HasGravity"": true,
    ""PositionOffset"": [0, 0, 0],
    ""Direction"": [0, 0.2, 1],
    ""Count"": 1,
    ""Speed"": 15.0,
    ""Duration"": 5.0,
    ""SpreadAngle"": 0.0,
    ""SpreadRange"": 0.0,
    ""ActivateOnCollision"": true,
    ""MaterialName"": ""Glow_Ice"",
    ""MeshName"": ""Crystal_4"",
    ""ParticleName"": ""Ef_IceMagicGlowFree01"",
    ""TrailName"": ""VFX_Trail_Ice"",
    ""Actions"": []
}

### 입력:
[PowerLevel: 2]
[CameraTargetPosition: 0.0, 0.0, 10.0]
[CasterPosition: 0.0, 0.0, 0.0]
상대방 머리 위에 바위 소환

### 출력:
{
    ""Name"": ""Stone Drop"",
    ""Element"": ""Earth"",
    ""Behavior"": ""Projectile"",
    ""Shape"": ""Cube"",
    ""Size"": [3, 3, 3],
    ""HasGravity"": true,
    ""PositionOffset"": [0.0, 5.0, 10.0], 
    ""Direction"": [0, -1, 0],
    ""Count"": 1,
    ""Speed"": 10.0,
    ""Duration"": 4.0,
    ""SpreadAngle"": 0.0,
    ""SpreadRange"": 0.0,
    ""ActivateOnCollision"": true,
    ""MaterialName"": ""M_Stones"",
    ""MeshName"": ""ST_Stone2"",
    ""ParticleName"": ""Sparks_green"",
    ""TrailName"": ""VFX_Trail_Earth"",
    ""Actions"": []
}
";



        private readonly string _apiKey;

        public TextToSpellApi()
        {
            if (!env.TryParseEnvironmentVariable("API_KEY", out _apiKey))
            {
                throw new ArgumentException("API key is not set in the environment variables.");
            }
        }

        // FIXME: cameraTargetPosition, casterPosition 없애기
        public async UniTask<string> TextToSpellAsync(string text, int powerLevel, Vector3 cameraTargetPosition, Vector3 casterPosition)
        {
            // 카메라 타겟 위치와 캐스터 위치 정보를 프롬프트에 추가
            var userPrompt =
                $"[PowerLevel: {powerLevel}]\n" +
                $"[CameraTargetPosition: {cameraTargetPosition.x}, {cameraTargetPosition.y}, {cameraTargetPosition.z}]\n" +
                $"[CasterPosition: {casterPosition.x}, {casterPosition.y}, {casterPosition.z}]\n" +
                $"{text}\n";

            var payload = new
            {
                model = Model,
                messages = new object[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = userPrompt }
                }
            }; // response_format 제거
            string requestJson = JsonConvert.SerializeObject(payload);

            using var request = new UnityWebRequest(ApiUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(requestJson));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

            var response = await request.SendWebRequest();

            if (response.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                return null;
            }

            var spellResponse = JsonConvert.DeserializeObject<SpellResponse>(response.downloadHandler.text);
            var json = spellResponse.Choices.First().Message.Content;

            // 마크다운 블록(```json ... ```) 제거
            if (json.StartsWith("```"))
            {
                int firstBrace = json.IndexOf('{');
                int lastBrace = json.LastIndexOf('}');
                if (firstBrace >= 0 && lastBrace > firstBrace)
                {
                    json = json.Substring(firstBrace, lastBrace - firstBrace + 1);
                }
            }

            return json;
        }

        public record SpellResponse(SpellResponse.Choice[] Choices)
        {
            public Choice[] Choices { get; } = Choices;

            public record Choice(Message Message)
            {
                public Message Message { get; } = Message;
            }

            public record Message
            {
                public string Content { get; }

                public Message(string content)
                {
                    Content = content;
                    Debug.Log("Api 요청 결과: " + content);
                }
            }
        }
    }
}
