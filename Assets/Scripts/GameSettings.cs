using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private int _collumnsNumber = 8;
    public int collumnsNumber {get {return _collumnsNumber;} private set {_collumnsNumber = value;}}
    [SerializeField] private int _dispencerRowsNumber = 2;
    public int dispencerRowsNumber {get {return _dispencerRowsNumber;} private set {_dispencerRowsNumber = value;}}
    [SerializeField] private int _fieldRowsNumber = 10;
    public int fieldRowsNumber {get {return _fieldRowsNumber;} private set {_fieldRowsNumber = value;}}
    [SerializeField] private float _collumnsDistance = 2.2f;
    public float collumnsDistance {get {return _collumnsDistance;} private set {_collumnsDistance = value;}}
    [SerializeField] private float _rowsDistance = 2f;
    public float rowsDistance {get {return _rowsDistance;} private set {_rowsDistance = value;}}
    [SerializeField] private float _speed = 50;
    public float speed {get {return _speed;} private set {_speed = value;}}
    [SerializeField] private int _mergingBallsCount = 5; //how many balls in one collumn of one color will merge in one
    public int mergingBallsCount {get {return _mergingBallsCount;} private set {_mergingBallsCount = value;}}
    [SerializeField] private int _burningBallsCount = 3; //how many balls in one row of one color will trigger burning
    public int burningBallsCount {get {return _burningBallsCount;} private set {_burningBallsCount = value;}}
    [SerializeField] private float _specialBallSpawnChance = 0.1f;
    public float specialBallSpawnChance {get {return _specialBallSpawnChance;} private set {_specialBallSpawnChance = value;}}
    [SerializeField] private int _maxWeightAdditionToLevel = 3;
    public int maxWeightAdditionToLevel {get {return _maxWeightAdditionToLevel;} private set {_maxWeightAdditionToLevel = value;}}
    [SerializeField] private int _colorsUsedAdditionToLevel = 3;
    public int colorsUsedAdditionToLevel {get {return _colorsUsedAdditionToLevel;} private set {_colorsUsedAdditionToLevel = value;}}
    [SerializeField] private int _startingLevel = 1;
    public int startingLevel {get {return _startingLevel;} private set {_startingLevel = value;}}
    [SerializeField] private int _ballsDroppedToLevelUp = 50;
    public int ballsDroppedToLevelUp {get {return _ballsDroppedToLevelUp;} private set {_ballsDroppedToLevelUp = value;}}
    [SerializeField] private float _upperPartOfScreen = 0.8f;
    public float upperPartOfScreen {get {return _upperPartOfScreen;} private set {_upperPartOfScreen = value;}}
    [SerializeField] private float _lowerPartOfScreen = 0.3f;
    public float lowerPartOfScreen {get {return _lowerPartOfScreen;} private set {_lowerPartOfScreen = value;}}
    [SerializeField] private float _leftAndRightBorderOfScreen = 0.5f;
    public float leftAndRightBorderOfScreen {get {return _leftAndRightBorderOfScreen;} private set {_leftAndRightBorderOfScreen = value;}}
    [SerializeField] private float _manipulatorReactivationTime = 0.5f;
    public float manipulatorReactivationTime {get {return _manipulatorReactivationTime;} private set {_manipulatorReactivationTime = value;}}

}
