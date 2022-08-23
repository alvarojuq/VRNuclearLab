using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadCalculation : MonoBehaviour
{
    [SerializeField] Transform radSource;
    [SerializeField] Transform shield;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform geigerTransform;
    [SerializeField] TextMeshProUGUI distanceRadSourceText;
    [SerializeField] TextMeshProUGUI distanceShieldText;
    [SerializeField] TextMeshProUGUI intensityText;
    [SerializeField] TextMeshProUGUI particleHitText;
    [SerializeField] TextMeshProUGUI LOSText;
    [SerializeField] TextMesh GeigerText;
    public ParticleSystem part;
    public GameObject LOS;

    [SerializeField] float radSourceRadius; // rad source radius
    [SerializeField] float radSourceEnergy; // A energy

    [SerializeField] float shieldThickness; // shield thickness length
    [SerializeField] float linearAttenuationCoefficient; // linear attenuation coefficient
    

    float distanceRad;
    float distanceShield;

    float distanceGeigerRad;
    float distanceGeigerShield;

    float areaOfRadSource; //4 * pi * r^2
    float IntensityO; // Intensity = A / Area
    float IntensityBehindShield;
    float IntensityAtDistance;
    float GeigerIntensityAtDistance;
    float Bq; // disintegration per sec
    float Ci; // 1 Ci = 3.7 x 10^10 Bq, Activity of the source

    // aluminum linear attenuation 0.136 cm -1
    /*
     *  **WORK IN PROGRESS**
     *  RAD Calculation Needs to be revised
     *  Needs to be compared to on paper calculations
    */


    // raycast version
    RaycastHit hit;

    List<ParticleCollisionEvent> collisionEvents;
    public float particleHits = 0;

    public bool playerIsLOS = false;

    public AudioClip collisionSound;

    // Start is called before the first frame update
    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        getLOS();

        distanceRad = Vector3.Distance(radSource.transform.position, playerTransform.transform.position);
        distanceShield = Vector3.Distance(shield.transform.position, playerTransform.transform.position);

        distanceGeigerRad = Vector3.Distance( radSource.transform.position , geigerTransform.transform.position );
        distanceGeigerShield = Vector3.Distance( shield.transform.position , geigerTransform.transform.position );

        RadActivityCalculation();

        // Distance equation
        // I1 = I2( X2^2 / X1^2 ) 
        // I2 = I1 (d1/d2)^2
        //D = T A t / d^2
        //D rate = T A d^2

        IntensityAtDistance = IntensityBehindShield * (Mathf.Pow(distanceRad, 2) / Mathf.Pow(distanceShield , 2));
        //IntensityAtDistance = ( radSourceEnergy ) / Mathf.Pow( distanceShield , 2 );

        GeigerIntensityAtDistance = IntensityBehindShield * ( Mathf.Pow( distanceGeigerRad , 2 ) / Mathf.Pow( distanceGeigerShield , 2 ) );


        if ( playerIsLOS ) {
            


            IntensityAtDistance = 100 * IntensityO * 1/( Mathf.Pow( distanceRad , 2 ) );
            //IntensityAtDistance = radSourceEnergy * ( Mathf.Pow( distanceRad , 2 ) );
            //IntensityAtDistance = (linearAttenuationCoefficient * radSourceEnergy) / ( Mathf.Pow( distanceRad , 2 ));
            //IntensityAtDistance = ( radSourceEnergy ) / Mathf.Pow( distanceRad , 2 );

            GeigerIntensityAtDistance = 100 * IntensityO * 1 / ( Mathf.Pow( distanceGeigerRad , 2 ) );


        }

        distanceRadSourceText.text = "Distance From Source: " + distanceRad.ToString("0.00") + " meters";
        distanceShieldText.text = "Distance From Shield: " + distanceShield.ToString("0.00") + " meters";
        intensityText.text = "Intensity : " + IntensityAtDistance.ToString("0.00") + " cpm";
        particleHitText.text = "Particle Hits Player " + particleHits.ToString( "0" ) + " Times";
        LOSText.text = "LOS " + playerIsLOS.ToString() + " ";
        GeigerText.text = GeigerIntensityAtDistance.ToString("0.0") + " cpm";
    }

    

    void RadActivityCalculation()
    {
        // Area = 4 * Pi * r^2
        areaOfRadSource = (4 * Mathf.PI * Mathf.Pow(radSourceRadius, 2));

        // I = A / Area
        IntensityO = radSourceEnergy / areaOfRadSource;

        // I = Io e^(-u*x)
        //IntensityBehindShield = radSourceEnergy * Mathf.Exp(-( (linearAttenuationCoefficient) * shieldThickness));
        IntensityBehindShield = IntensityO * Mathf.Exp( -( linearAttenuationCoefficient * shieldThickness ) );
    }

    void OnParticleCollision( GameObject other ) {

        if ( other.tag == "Player" ) {
            Debug.Log( other.tag );
            particleHits++;

            AudioSource.PlayClipAtPoint( collisionSound , other.transform.position );
        }

    }

    void getLOS() {
        playerIsLOS = LOS.GetComponent<LOS>().GetLOS();
    }

    


}
