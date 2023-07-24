using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class personalityTest : MonoBehaviour
{
    // each question is stored as a State, with all info about that question (next states, answers, etc)
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

        // show question & answer text
        public void printText()
        {
            q_text.text = q;
            a0_text.text = "(A) " + a0;
            a1_text.text = "(B) " + a1;
        }

        // return next state based on which answer is chosen
        public int nextState(int btn_pressed)
        {
            return ((btn_pressed == 0) ? next_st_0 : next_st_1);
        }
    };

    // the 11 different end states are also stored as structs
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

        // activate final scene & corresponding text
        public void endScreen()
        {
            cube.SetActive(true);
            end_title_text.text = "You got: " + title;
            end_body_text.text = body_text;
        }

        // hides cube from this particular end state
        public void deactivateCube()
        {
            cube.SetActive(false);
        }
    }
    
    // All UI text & objects
    static TMP_Text q_text, a0_text, a1_text, end_title_text, end_body_text;
    public GameObject start_ui, question_ui, answer_0_ui, answer_1_ui, end_ui, end_frame;
    public GameObject end_cube_1, end_cube_2, end_cube_3, end_cube_4, end_cube_5, end_cube_6, end_cube_7, end_cube_8, end_cube_9, end_cube_10, end_cube_11;

    // Stuff to control the various state machines
    private int state, num_questions;
    private bool started, ended;

    // stack to record previously visited states (for undo button)
    private Stack<int> prev_states = new Stack<int>();

    // arrays to hold states
    private State[] state_array;
    private endState[] end_state_array;
    

    void Start()
    {
        // get text from various UI panels (has to happen in start bc of the structs)
        q_text = question_ui.transform.GetChild(0).GetComponent<TMP_Text>();
        a0_text = answer_0_ui.transform.GetChild(0).GetComponent<TMP_Text>();
        a1_text = answer_1_ui.transform.GetChild(0).GetComponent<TMP_Text>();
        end_title_text = end_ui.transform.GetChild(1).GetComponent<TMP_Text>();
        end_body_text = end_ui.transform.GetChild(2).GetComponent<TMP_Text>();

        // initialize state array with all states & text
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

        // initialize end state array
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

        // test prep stuff
        num_questions = state_array.Length;
        startTest();
    }

    void Update()
    {
        if (ended)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.Y))
            {
                // restart test if ended & restart button pressed
                StartCoroutine(restartTest());
            }
            else return;
        }
        else if (started)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A))
            {
                // first answer chosen, go to next question
                StartCoroutine(changeStates(0));
            }
            else if (OVRInput.GetDown(OVRInput.RawButton.B))
            {
                // second answer chosen, go to next question
                StartCoroutine(changeStates(1));
            }
            else if (OVRInput.GetDown(OVRInput.RawButton.X))
            {
                // back button chosen
                StartCoroutine(changeStates(2));
            }
        }
        else if (!started && OVRInput.GetDown(OVRInput.RawButton.A))
        {
            // begin test (from start screen)
            StartCoroutine(enterTest());
        }
    }

    IEnumerator pressButton(Transform button) 
    {
        // simple button press color animation (turns green for 0.5 seconds)

        button.GetComponent<Image>().color = new Color32(0,166,17,174);

        yield return new WaitForSeconds(0.5f);
            
        button.GetComponent<Image>().color = new Color32(0,0,0,174);
    }


    void startTest()
    {
        // configure UI panel for starting screen
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
        
        // configure UI for beginning test (going to first question)
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
            // if redo, go back to previous state
            yield return pressButton(question_ui.transform.GetChild(1));

            state = prev_states.Pop();
            state_array[state - 1].printText();
        }
        else 
        {
            // get pressed button
            Transform btn_bkg = (btn_press == 0) ? answer_0_ui.transform : answer_1_ui.transform;
            yield return pressButton(btn_bkg);

            // add state to stack & move to next one
            prev_states.Push(state);
            state = state_array[state - 1].nextState(btn_press);

            if (state > num_questions)
            {
                // if at the end of the test, go to ending screen
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
        // configure UI for end of test
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

        // erase answer cube & restart test
        end_state_array[state - 1 - num_questions].deactivateCube();
        
        startTest();
    }
}
