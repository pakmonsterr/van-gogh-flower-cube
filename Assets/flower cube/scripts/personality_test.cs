using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class personality_test : MonoBehaviour
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
    
    static TMP_Text q_text;
    static TMP_Text a0_text;
    static TMP_Text a1_text;

    public GameObject start;
    public GameObject question;
    public GameObject answer_0;
    public GameObject answer_1;

    private State[] state_array;
    private int state;
    private Stack<int> prev_states = new Stack<int>();

    private bool started = false;
    
    // Start is called before the first frame update
    void Start()
    {
        q_text = question.transform.GetChild(0).GetComponent<TMP_Text>();
        a0_text = answer_0.transform.GetChild(0).GetComponent<TMP_Text>();
        a1_text = answer_1.transform.GetChild(0).GetComponent<TMP_Text>();

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
                                        "So you might be going to kail for violent crime?",
                                        "Uh... yes?",
                                        "I need to inject morphine IN MY EYE")
                                    };
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            if (started)
            {
                changeStates(0);
            }
            else
            {
                start.SetActive(false);
                answer_0.SetActive(true);
                answer_1.SetActive(true);

                state = 1;
                state_array[state - 1].printText();
                
                started = true;
            }
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.B) && started)
        {
            changeStates(1);
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.X) && started)
        {
            changeStates(2);
        }
    }

    void changeStates(int btn_press)
    {
        if (btn_press == 2)
        {
            state = prev_states.Pop();
            state_array[state - 1].printText();
        }
        else 
        {
            prev_states.Push(state);
            state = state_array[state - 1].nextState(btn_press);
            state_array[state - 1].printText();
        }
    }
}
