# [Unity 3D] BoardGame
## 1. 소개

<div align="center">
  <img src="https://github.com/k660323/BoardGame/blob/main/Images/%EB%A1%9C%EA%B7%B8%EC%9D%B8%20%ED%99%94%EB%A9%B4.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/BoardGame/blob/main/Images/%EB%A9%94%EC%9D%B8%20%ED%99%94%EB%A9%B4.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/BoardGame/blob/main/Images/%EC%98%B5%EC%85%98.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/BoardGame/blob/main/Images/%EC%83%81%EC%A0%90.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/BoardGame/blob/main/Images/%EC%9D%B8%EB%B2%A4%ED%86%A0%EB%A6%AC.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/BoardGame/blob/main/Images/%EB%B0%A9%20%EC%B0%BE%EA%B8%B0.JPG" width="49%" height="300"/>
  <img src="https://github.com/k660323/BoardGame/blob/main/Images/%EB%B0%A9.JPG" width="99%" height="600"/>
  <img src="https://github.com/k660323/BoardGame/blob/main/Images/%EA%B2%8C%EC%9E%84%20%ED%99%94%EB%A9%B4.JPG" width="99%" height="600"/>
  
  < 게임 플레이 사진 >
</div>

+ BoardGame이란?
  + 멀티플레이 캐주얼 보드 게임 입니다.
 
+ 목표
  + 라인독점, 관광지 독점 또는 상대팀을 파산 시켜 승리하세요.

+ 게임 흐름
  + 게임 시작시 정해진 시간안에 숫자가 적힌 카드를 선택해주시면 됩니다. (주사위를 굴릴 순서)
  + 그 후 내차례에 주사위를 굴러 나온숫자만큼 움직입니다.
  + 해당 땅에 도착했을시
  + 일반 지역이면 땅을 살 수 있다. 만약 상대팀 땅이면 통행료를 지불하고 땅을 매입할 수 있다. 만약 통행료가 없으면 파산처리가 된다. 특수 지역이면 설정된 이벤트 발생
  + 이 흐름이 반복되어 라인 독점, 관광지 독점, 또는 상대팀 파산을 시켜 이길 수 있다.        

<br>

## 2. 프로젝트 정보

+ 사용 엔진 : UNITY
  
+ 엔진 버전 : 2020.3.19f1 LTS

+ 사용 언어 : C#
  
+ 작업 인원 : 1명
  
+ 작업 영역 : 콘텐츠 제작, 디자인, 기획
  
+ 장르      : 보드 게임
  
+ 소개      : Photon 에셋을 활용하여 만든 멀티플레이 캐주얼 보드 게임이다.
  
+ 플랫폼    : PC
  
+ 개발기간  : 2020.02.03 ~ 2021.01.11
  
+ 형상관리  : GitHub Desktop

<br>

## 3. 사용 기술
| 기술 | 설명 |
|:---:|:---|
| Save | 게임 데이터를 Json형태로 저장 |
| Load | 게임 데이터를 Json형태로 불러온다.|
| Photon | OpenAPI 네트워크 에셋으로 통해 손쉽게 네트워크 기능 구현
<br>

<br>

---

<br>

## 4. 구현에 어려웠던 점과 해결과정
+ P2P 네트워크 구조에 대한 이해가 부족해서 동기화가 잘 되지 않았습니다.
  + Photon 공식 홈페이지나 Youtube 영상을 참고하거나 관련 있는 지식인 분에게 질문을 해서 문제 해결 및 구현하였습니다.
  
+ Solid원칙을 지키지 않아 코드가 복잡하고 이해하기 어렵고 확장 및 설계에 하는데 많은 어려움이 있었고 개발 시간이 많이 지연되었습니다.
  + 추후 정보처리기사 자격증을 준비하면서 Solid원칙을 이해하고 추후 포트폴리오에서 이 원칙을 지키도록 노력하였습니다.
    
+ 데이터를 모두 수작업으로 유니티 인스펙터 창 에서 설정 하는 게 힘들었습니다. 
  + 추후 다른 포트폴리오에서 데이터를 Json으로 만든 다음 역 직렬화를 해서 데이터를 불러오도록 하여 수작업 하던 것 들을 간편하게 불러오고 수정하도록 구현하였습니다.
 
## 5. 느낀점
+ Zombie Suriver보다는 코드가 정리되고 간결화 되었지만 아직 부족하다 객체간 접근이 불편하고 너무 나눠져 있어서 불편하다. 다음 프로젝트에는 매니저를 둬서 명확하게 관리의 필요성을 느낌
+ 매번 프로젝트할때 처음부터 만들면 시간이 많이 걸리고 코드 편의성도 많이 떨어져 템플릿 프로젝트가 필요함을 느낌
+ Photon Asset와 네트워크 프로그래밍 이해도가 낮다고 느낌 좀 더 문서와 네트워크 공부가 필요함

## 6. 플레이 영상
+ https://www.youtube.com/watch?v=VdKAyiOhw1c
