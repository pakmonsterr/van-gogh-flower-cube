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
            end_title_text.text = title;
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
    public GameObject end_cube_1, end_cube_2, end_cube_3, end_cube_4, end_cube_5, end_cube_6;

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
                                        "여러가지 생각이 많은 편이다",
                                        "네",
                                        "아니요"), 
                                    new State(2, 3, 5,
                                        "소통을 잘 한다",
                                        "네",
                                        "아니요"),
                                    new State(3, 19, 6,
                                        "새로운 지식을 배우는 것이 즐겁다",
                                        "네",
                                        "아니요"),
                                    new State(4, 5, 7,
                                        "자기관리가 어렵다",
                                        "네",
                                        "아니요"),
                                    new State(5, 6, 8,
                                        "에너지가 넘친다",
                                        "네",
                                        "아니요"),
                                    new State(6, 20, 9,
                                        "인간관계가 넓다",
                                        "네",
                                        "아니요"),
                                    new State(7, 8, 10,
                                        "감정 컨트롤을 잘하는 편이다",
                                        "네",
                                        "아니요"),
                                    new State(8, 9, 11,
                                        "공감능력이 좋다",
                                        "네",
                                        "아니요"),
                                    new State(9, 21, 12,
                                        "조용한 성격이다",
                                        "네",
                                        "아니요"),
                                    new State(10, 11, 13,
                                        "협업 보다 혼자하는 게 편하다",
                                        "네",
                                        "아니요"),
                                    new State(11, 12, 14,
                                        "반복적인 것이 싫다",
                                        "네",
                                        "아니요"),
                                    new State(12, 22, 15,
                                        "아이디어가 많다",
                                        "네",
                                        "아니요"),
                                    new State(13, 14, 16,
                                        "의견을 말하는데 어려움이 없다",
                                        "네",
                                        "아니요"),
                                    new State(14, 15, 17,
                                        "리더쉽이 있다",
                                        "네",
                                        "아니요"),
                                    new State(15, 23, 18,
                                        "맡은 일에 책임감이 강하다",
                                        "네",
                                        "아니요"),
                                    new State(16, 17, 14,
                                        "나는 나보다 남을 더 신경쓴다",
                                        "네",
                                        "아니요"),
                                    new State(17, 18, 15,
                                        "난 운명적인 사랑을 믿는다",
                                        "네",
                                        "아니요"),
                                    new State(18, 24, 23,
                                        "감수성이 풍부하다",
                                        "네",
                                        "아니요"),
                                    };

        // initialize end state array
        end_state_array = new endState[] {  new endState(end_cube_1,
                                                "균형과 신중함을 가진 붓꽃속",
                                                "text"),
                                            new endState(end_cube_2,
                                                "언제나 밝고 심지 굳은 해바라기",
                                                "text"),  
                                            new endState(end_cube_3,
                                                "차분하고 우아한 흰장미",
                                                "text"),  
                                            new endState(end_cube_4,
                                                "개성적이고 독특한 양귀비",
                                                "text"),  
                                            new endState(end_cube_5,
                                                "강인하고 용감한 글라디올러스",
                                                "text"),  
                                            new endState(end_cube_6,
                                                "6. 섬세하고 로맨틱한 아몬드꽃",
                                                "text")
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
