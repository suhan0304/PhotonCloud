using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks // PUN의 다양한 콜백 함수를 오버라이드해서 작성
{
    private readonly string version = "1.0";

    private string userId = "Zack";

    // 유저명을 입력할 TMP Input Field
    public TMP_InputField userIF;
    // 룸 이름으 ㄹㄹ입력할 TMP Input Field
    public TMP_InputField roomNameIF;


    void Awake() {
        // 마스터 클라이언트 씬 자동 동기화 옵션
        PhotonNetwork.AutomaticallySyncScene = true;
        // 게임 버전 설정
        PhotonNetwork.GameVersion = version;
        // 접속 유저의 닉네임 설정
        PhotonNetwork.NickName = userId;

        // 포톤 서버와의 데이터의 초당 전송 횟수
        Debug.Log(PhotonNetwork.SendRate);

        // 포톤 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start() {
        // 저장된 유저명을 로드
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1,21):00}");
        userIF.text = userId;
        // 접속 유저의 닉네임 등록
        PhotonNetwork.NickName = userId;
    }

    // 유저명을 설정하는 로직
    public void SetUserID() {
        if (string.IsNullOrEmpty(userIF.text)) {
            userId = $"USER_{Random.Range(1,21):00}";
        }
        else {
            userId = userIF.text;
        }

        // 유저명 저장
        PlayerPrefs.SetString("USER_ID", userId);
        // 접속 유저의 닉네임 등록
        PhotonNetwork.NickName = userId;
    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    string SetRoomName() {
        if (string.IsNullOrEmpty(roomNameIF.text)) {
            roomNameIF.text = $"ROOM_{Random.Range(1,101):000}";
        }
        return roomNameIF.text;
    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster() {
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    // 로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby() {
        Debug.Log($"PhotonNetwork.InLobby {PhotonNetwork.InLobby}");
        //PhotonNetwork.JoinRandomRoom(); // 수동 접속
    }

    // 랜덤한 룸 입장이 실패했을 때 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");
        // 룸을 생성하는 함수 실행
        OnMakeRoomClick();

        /*
        // 룸의 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;     // 최대 접속자 수
        ro.IsOpen = true;       // 룸의 오픈 여부
        ro.IsVisible = true;    // 로비에서 룸을 노출시킬지 여부

        // 룸 생성
        PhotonNetwork.CreateRoom("My Room", ro);
        */
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        /*
        foreach(var player in PhotonNetwork.CurrentRoom.Players) {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        // 출현 위치 정보를 배열에 저장
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        // 네트워크상에 캐릭터 생성
        PhotonNetwork.Instantiate("player", points[idx].position, points[idx].rotation, 0);
        */

        // 마스터 클라이언트인 경우에 룸에 입장한 후 전투 씬을 로딩한다.
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel("BattleField");
        }
    }

#region UI_BUTTON_EVENT

    public void OnLoginClick() {
        // 유저명 저장
        SetUserID();

        // 무작위로 추출한 룸으로 입장
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick() {
        // 유저명 저장
        SetUserID();

        // 룸의 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;     // 최대 접속자 수
        ro.IsOpen = true;       // 룸의 오픈 여부
        ro.IsVisible = true;    // 로비에서 룸을 노출시킬지 여부

        // 룸 생성
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }
#endregion

}

