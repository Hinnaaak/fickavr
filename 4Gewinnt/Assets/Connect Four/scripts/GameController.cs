using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GameController : MonoBehaviour {

    public enum Player
    {
        Empty = 0,
        Yellow = 1,
        Red = 2
    }

    public GameObject redCoin;
    public GameObject yellowCoin;
    public GameObject currentCoin;

    public int row = 6;
    public int col = 7;
    public int[,] field;

    public bool isDropping;
    public Player turn;
    public Player me;

    public float speed = 30f;

    private Vector3 endPosition;
    public bool isLocked;
    public Player winner;

    public bool blockMovingCoin = false;

    public string tag;

    // Use this for initialization
    void Start () {
    
        field = new int[col, row];
        turn = Player.Red;
        winner = Player.Empty;
        CreateNewCoin();
    }

    // Update is called once per frame
    void Update () {
        if (!isDropping && !isLocked && currentCoin != null && isMyTurn())
        {
            isLocked = true;
            Vector3 moveFor = new Vector3(0, 0, 0);
            Transform playerGlobal = GameObject.Find("PlayerController").transform;
            Transform playerLocal = playerGlobal.Find("[VRTK_SDKManager]/SDKSetups/OculusVR/OVRCameraRig/TrackingSpace/CenterEyeAnchor");
            Vector3 position = playerLocal.position;

            if ((Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") < 0) && !blockMovingCoin && currentCoin.transform.position.z < 0.75f && position.x < 0.0f)
            {
                //if(position.x < 0.0f)
                moveFor = new Vector3(0, 0, 0.01f);
                //else
                //    moveFor = new Vector3(0, 0, -0.01f);
                //blockMovingCoin = true;
            }
            if ((Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") < 0) && !blockMovingCoin && currentCoin.transform.position.z > -0.75f && position.x > 0.0f)
            {
                //if (position.x < 0.0f)
                //
                // else
                moveFor = new Vector3(0, 0, -0.01f);
                //blockMovingCoin = true;
            }

            if ((Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") > 0) && currentCoin.transform.position.z > -0.75f && !blockMovingCoin && position.x < 0.0f)
            {
                //if (position.x < 0.0f)
                moveFor = new Vector3(0, 0, -0.01f);
               // else
                 //   moveFor = new Vector3(0, 0, 0.01f);
                //blockMovingCoin = true;
            }
            if ((Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") > 0) && currentCoin.transform.position.z < 0.75f && !blockMovingCoin && position.x > 0.0f)
            {

                // if (position.x < 0.0f)
                //    moveFor = new Vector3(0, 0, -0.01f);
                //else
                moveFor = new Vector3(0, 0, 0.01f);
                //blockMovingCoin = true;
            }
            currentCoin.transform.position += moveFor;
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickHorizontal") == 0)
                blockMovingCoin = false;
            if (Input.GetButtonDown("Oculus_CrossPlatform_Button2"))
             {
                 DropCoin();
             }
            isLocked = false;
        }
        else if(isDropping)
        {
            if(currentCoin.transform.position.y < 1.5f)
            {
                isDropping = false;
                turn = turn == Player.Red ? Player.Yellow : Player.Red;
                currentCoin = null;
                CheckForWinner();
                if (winner == Player.Empty && HasEmptyCell())
                    CreateNewCoin();
                collision childScript = GameObject.FindWithTag(tag).transform.GetComponent<collision>();

                childScript.unblockCollision();

                Debug.Log("Next please");
            }

            /*Vector3 move = new Vector3(0, -0.75f, 0) * Time.deltaTime * speed;
    if (currentCoin.transform.position.y > endPosition.y){
        if((move + currentCoin.transform.position).y >= endPosition.y)
            currentCoin.transform.Translate(move, Space.World);
        else
            currentCoin.transform.Translate(new Vector3(0, endPosition.y - currentCoin.transform.position.y, 0), Space.World);
    }else{
        isDropping = false;
        turn = turn == Player.Red ? Player.Yellow : Player.Red;
        CheckForWinner();
        currentCoin = null;
        if(winner == Player.Empty && HasEmptyCell())
            CreateNewCoin();
    }*/
        }

    }

    void CreateNewCoin(){
        //Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject g = Instantiate(
            turn == Player.Red ? redCoin : yellowCoin , // is players turn = spawn red, else spawn yellow
            new Vector3(0, 2.02f, 0), // spawn it above the first row
            Quaternion.Euler(new Vector3(0, 0, 90)));
        currentCoin = g;
    }

    private int GetCol(){
           return (int)Mathf.Floor(currentCoin.transform.position.z / -2.5f) + 3;
    }

    public void DropCoin(){
        /* isDropping = true;

         Vector3 startPosition = currentCoin.transform.position;
         endPosition = new Vector3(0, 1.5f, 0);

         // is there a free cell in the selected column?
         bool foundFreeCell = false;
         for (int i = row - 1; i >= 0; i--){
             if (field[GetCol(), i] == 0){
                 foundFreeCell = true;
                 endPosition += new Vector3(0, 2.5f, 0)*(row -1- i);
                 field[GetCol(), i] = (int)turn;
                 break;
             }

         }
         if (foundFreeCell){
             currentCoin.transform.Translate(Vector3.down * Time.deltaTime * speed, Space.World);
         }else
             isDropping = false;
             */

        currentCoin.GetComponent<Rigidbody>().isKinematic = false;
        //currentCoin = null;
    }

    private void CheckForWinner(){
        Debug.Log("Suche Nach Gewinner");
        for (int i = row - 1; i >= 0; i--){
            for (int j = col - 1; j >= 0; j-- ){
                if(field[j,i] != (int)Player.Empty){
                    int owner = field[j, i];

                    //Check 4 from down to top
                    int count = 0;
                    for (int x = i; x > i-4; x--){
                        if (IsInField(j,x) && field[j, x] == owner)
                        {
                            count++;
                        }else{
                            count = 0;
                            break;
                        }
                    }
                    if (count == 4){
                        winner = (Player)owner;
                        Debug.Log("4 in einer reihe down-top. Gewonnen hat Spieler: " + winner);
                        return;
                    }

                    //check 4 from right to left
                    for (int x = j; x > j-4 ; x--)
                    {
                        if (IsInField(x,i) && field[x, i] == owner)
                            count++;
                        else
                        {
                            count = 0;
                            break;
                        }
                    }
                    if (count == 4)
                    {
                        winner = (Player)owner;
                        Debug.Log("4 in einer reihe right-left. Gewonnen hat Spieler: " + winner);
                        return;
                    }

                    //Check 4 from left top to right down
                    for (int x = 0; x < 4; x++)
                    {
                        if (IsInField(j+x, i+x) && field[j+x, i+x] == owner)
                            count++;
                        else
                        {
                            count = 0;
                            break;
                        }
                    }
                    if (count == 4)
                    {
                        winner = (Player)owner;
                        Debug.Log("4 in einer reihe left top-right down. Gewonnen hat Spieler: " + winner);

                        return;
                    }

                    //Check 4 from right top to left down
                    for (int x = 0; x < 4; x++)
                    {
                        if (IsInField(j + x, i - x) && field[j + x, i - x] == owner)
                            count++;
                        else
                        {
                            count = 0;
                            break;
                        }
                    }
                    if (count == 4)
                    {
                        winner = (Player)owner;
                        Debug.Log("4 in einer reihe right top-left down. Gewonnen hat Spieler: " + winner);
                        return;
                    }
                }
            }
        }
    }

    private bool IsInField(int x, int y)
    {
        return x > -1 && y > -1 && x < col && y < row;
    }

    private bool HasEmptyCell()
    {
        for (int i = row - 1; i >= 0; i--)
        {
            for (int j = col - 1; j >= 0; j--)
            {
                if (field[j, i] == (int)Player.Empty){
                    return true;
                }
            }
        }
        return false;
    }

    public void CollisionFromChild(string col, string coin)
    {
        isDropping = true;
        tag = col;
        int collumn = 0;
        if (col == "1")
            collumn = 1;
        else if (col == "2")
            collumn = 2;
        else if (col == "3")
            collumn = 3;
        else if (col == "4")
            collumn = 4;
        else if (col == "5")
            collumn = 5;
        else if (col == "6")
            collumn = 6;
        // is there a free cell in the selected column?
        for (int i = row - 1; i >= 0; i--)
        {
            if (field[collumn, i] == 0)
            {
                endPosition += new Vector3(0, 2.5f, 0) * (row - 1 - i);
                field[collumn, i] = (int)(coin == "redCoin(Clone)" ? Player.Red: Player.Yellow);
                break;
            }

        }

        Debug.Log("Child collision verarbeitet");
    }

    public void setPlayer(int player)
    {
        switch (player){
            case 1:
                me = Player.Red;
                break;
            case 2:
                me = Player.Yellow;
                break;
            default:
                me = Player.Empty;
                break;
        }
        Debug.Log("Ich bin: " + me);
    }
    public bool isMyTurn()
    {
        return this.turn == this.me;
    }

    public void coinMove(Vector3 coinPos)
    {
        currentCoin.transform.position = coinPos;
    }

    public Vector3 getPlayerPosition()
    {
        Transform playerGlobal = GameObject.Find("PlayerController").transform;
        Transform playerLocal = playerGlobal.Find("[VRTK_SDKManager]/SDKSetups/OculusVR/OVRCameraRig/TrackingSpace/CenterEyeAnchor");
        return playerLocal.position;
    }


    public Vector3 getCoinPos()
    {
        return this.currentCoin.transform.position;
    }
}
