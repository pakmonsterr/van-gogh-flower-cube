using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class personalityTest : MonoBehaviour
{
    public struct State
    {
        public State(int state_no, int to_0_state, int to_1_state, string question, string ans_0, string ans_1)
        {
            q = question;
            a0 = ans_0;
            a1 = ans_1;

            current_st = state_no;
            next_st_0 = to_0_state;
            next_st_1 = to_1_state;
        }

        public string q { get; }
        public string a0 { get; }
        public string a1 { get; }

        public int current_st { get; }
        public int next_st_0 { get; }
        public int next_st_1 { get; }

        public void printText()
        {
            q_text.text = q;
            a0_text.text = "(A) " + a0;
            a1_text.text = "(B) " + a1;
        }

        public int nextState(int btn_pressed)
        {
            return ((btn_pressed == 0) ? next_st_0 : next_st_1);
        }
    };

    public struct endState
    {
        public endState(GameObject end_cube, string end_title, string end_text)
        {
            cube = end_cube;
            title = end_title;
            body_text = end_text;
        }

        public GameObject cube { get; }
        public string title { get; }
        public string body_text { get; }

        public void endScreen()
        {
            cube.SetActive(true);
            end_title_text.text = "You got: " + title;
            end_body_text.text = body_text;
        }

        public void deactivateCube()
        {
            cube.SetActive(false);
        }
    }
    
    static TMP_Text q_text;
    static TMP_Text a0_text;
    static TMP_Text a1_text;
    static TMP_Text end_title_text;
    static TMP_Text end_body_text;

    public GameObject start_ui;
    public GameObject question_ui;
    public GameObject answer_0_ui;
    public GameObject answer_1_ui;
    public GameObject end_ui;
    public GameObject end_frame;

    private State[] state_array;
    private int state;
    private Stack<int> prev_states = new Stack<int>();
    private int num_questions;
    private endState[] end_state_array;

    private bool started;
    private bool ended;

    public GameObject end_cube_1;
    public GameObject end_cube_2;
    public GameObject end_cube_3;
    public GameObject end_cube_4;
    public GameObject end_cube_5;
    public GameObject end_cube_6;
    public GameObject end_cube_7;
    public GameObject end_cube_8;
    public GameObject end_cube_9;
    public GameObject end_cube_10;
    public GameObject end_cube_11;
    
    // Start is called before the first frame update
    void Start()
    {
        q_text = question_ui.transform.GetChild(0).GetComponent<TMP_Text>();
        a0_text = answer_0_ui.transform.GetChild(0).GetComponent<TMP_Text>();
        a1_text = answer_1_ui.transform.GetChild(0).GetComponent<TMP_Text>();
        end_title_text = end_ui.transform.GetChild(1).GetComponent<TMP_Text>();
        end_body_text = end_ui.transform.GetChild(2).GetComponent<TMP_Text>();


        state_array = new State[] { new State(1, 2, 4,
                                        "When confronted in a dark alley, your first inclination is to...",
                                        "Run away",
                                        "Leave no survivors"), 
                                    new State(2, 5, 6,
                                        "Are you in shape?",
                                        "*Gasp* sorry I'm still winded from that question",
                                        "Does yoga count?"), 
                                    new State(3, 6, 7,
                                        "Ever see a Kung-fu movie?",
                                        "Does \"Rush Hour 3\" count?",
                                        "Yes"), 
                                    new State(4, 3, 7,
                                        "How many Red Bulls have you consumed in the past hour?",
                                        "None, way too much sugar",
                                        "My brain is shaking right now"), 
                                    new State(5, 8, 15,
                                        "Who would you base your fighting style on?",
                                        "Star Wars kid",
                                        "Battle-Star Galactica kid"), 
                                    new State(6, 9, 10,
                                        "You consider Vin Diesel to be a...",
                                        "Father figure",
                                        "Worthy opponent"),
                                    new State(7, 10, 6,
                                        "Ever heard of creatine?",
                                        "I HAVE SO MUCH RAGE",
                                        "No"),
                                    new State(8, 15, 11,
                                        "Have you ever been in a fight?",
                                        "You mean on Xbox Live?",
                                        "Yes"),
                                    new State(9, 8, 12,
                                        "Are you referring to Vin Diesel in \"The Pacifier?\"",
                                        "Yes",
                                        "No way"),
                                    new State(10, 14, 12,
                                        "Are your hands currently covered in blood?",
                                        "... kind of",
                                        "No"),
                                    new State(11, 13, 13,
                                        "Was it against a baby?",
                                        "Yes",
                                        "No"),
                                    new State(12, 18, 17,
                                        "Do you have any regard for sports, achievement, or integrity?",
                                        "Yes",
                                        "No"),
                                    new State(13, 16, 15,
                                        "Did you win?",
                                        "Duh",
                                        "... no"),
                                    new State(14, 18, 19,
                                        "So you might be going to jail for violent crime?",
                                        "Uh... yes?",
                                        "I need to inject morphine IN MY EYE")
                                    };

        end_state_array = new endState[] {  new endState(end_cube_1,
                                                "No one",
                                                "You are literally the worst fighter on the planet, even behind the 500 million people under the age of three. You should be terrified of everything and everyone."),
                                            new endState(end_cube_2,
                                                "A baby",
                                                "Ok so you can only beat up a baby. If you're also a baby, congrats! If not, maybe lay low for a while until this blows over."),  
                                            new endState(end_cube_3,
                                                "Coked-out 80's pro wrestler",
                                                "While more impressive than a baby, these washed u nobodies would die on their own if they weren't constantly monitored by a team of medical professionals, so don't get on your high horse."),  
                                            new endState(end_cube_4,
                                                "Football player",
                                                "You can beat up a football player, but he plays for the Buffalo Bills, so no one's impressed."),  
                                            new endState(end_cube_5,
                                                "A sizeable group of innocent people",
                                                "Just stay away from me! Are you in the room? I can hear someone breathing. ARE YOU IN HERE? Oh god the lights just went off and I--"),  
                                            new endState(end_cube_6,
                                                "",
                                                ""),
                                            new endState(end_cube_7,
                                                "",
                                                ""),  
                                            new endState(end_cube_8,
                                                "",
                                                ""),  
                                            new endState(end_cube_9,
                                                "",
                                                ""),  
                                            new endState(end_cube_10,
                                                "",
                                                ""),  
                                            new endState(end_cube_11,
                                                "",
                                                "")
                                        };    

        num_questions = state_array.Length;

        startTest();
    }

    // Update is called once per frame
    void Update()
    {
        if (ended)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.Y))
            {
                StartCoroutine(restartTest());
            }
            else return;
        }
        else if (started)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A))
            {
                StartCoroutine(changeStates(0));
            }
            else if (OVRInput.GetDown(OVRInput.RawButton.B))
            {
                StartCoroutine(changeStates(1));
            }
            else if (OVRInput.GetDown(OVRInput.RawButton.X))
            {
                StartCoroutine(changeStates(2));
            }
        }
        else if (!started && OVRInput.GetDown(OVRInput.RawButton.A))
        {
            StartCoroutine(enterTest());
        }
    }

    IEnumerator pressButton(Transform button) 
    {
        button.GetComponent<Image>().color = new Color32(0,166,17,174);

        yield return new WaitForSeconds(0.5f);
            
        button.GetComponent<Image>().color = new Color32(0,0,0,174);
    }


    void startTest()
    {
        start_ui.SetActive(true);
        answer_0_ui.SetActive(false);
        answer_1_ui.SetActive(false);
        question_ui.SetActive(false);
        end_ui.SetActive(false);
        end_frame.SetActive(false);
        
        started = false;
        ended = false;

        state = 1;
    }

    IEnumerator enterTest()
    {
        yield return pressButton(start_ui.transform);
        
        start_ui.SetActive(false);
        answer_0_ui.SetActive(true);
        answer_1_ui.SetActive(true);
        question_ui.SetActive(true);
        end_ui.SetActive(false);
        end_frame.SetActive(false);

        started = true;
        ended = false;

        state_array[state - 1].printText();
    }

    IEnumerator changeStates(int btn_press)
    {
        if (btn_press == 2)
        {
            yield return pressButton(question_ui.transform.GetChild(1));

            state = prev_states.Pop();
            state_array[state - 1].printText();
        }
        else 
        {
            Transform btn_bkg = (btn_press == 0) ? answer_0_ui.transform : answer_1_ui.transform;
            yield return pressButton(btn_bkg);

            prev_states.Push(state);
            state = state_array[state - 1].nextState(btn_press);

            if (state > num_questions)
            {
                endTest();
            }
            else
            {
                state_array[state - 1].printText();
            }
        }
    }

    void endTest()
    {
        start_ui.SetActive(false);
        answer_0_ui.SetActive(false);
        answer_1_ui.SetActive(false);
        question_ui.SetActive(false);
        end_frame.SetActive(true);
        end_ui.SetActive(true);

        started = true;
        ended = true;

        end_state_array[state - 1 - num_questions].endScreen();
    }

    IEnumerator restartTest()
    {
        yield return pressButton(end_ui.transform.GetChild(3));
                
        end_state_array[state - 1 - num_questions].deactivateCube();
        
        startTest();
    }
}
