import { ChatMessage } from "./blakulla-service";

export default class GameState {    
    static from(state: Readonly<GameState>): GameState {
        let self = <any>this;
        let other = <any>state;
        const s = new GameState();
        for (let prop in state) {  
            self[prop] = other[prop];
        }
        return s;
    }
    
    // [key: string]: any;
    
    public chatMessages: ChatMessage[] = [];

    public game: GameInfo = null;
    public theme: string = "";

    public name: string = "";
    public role: string = "";
    public joined: boolean = false;    
    public state: number = -1;
    public lynched: boolean = false;

    public isGameReady: boolean = false;

    public isVisible: boolean = false;    
    public waitForJoin: boolean = false;
    public waitForLeave: boolean = false;
    public waitForVote: boolean = false;
    public waitForAbility: boolean = false;
}

export class GameInfo {
    // {subPhaseDuration: 30, subPhaseStart: "2019-04-06T22:59:03.4091534Z", subPhase: "Voting", phase: "Day", playerCount: 8}
    public players: Player[] = [];
    public phase: string = null;
    public subPhase: string = null;
    public subPhaseStart: Date = null;
    public subPhaseDuration:number = 0;
}

export class Player {
    public name: string = null;
    public lynched: boolean = false;
}