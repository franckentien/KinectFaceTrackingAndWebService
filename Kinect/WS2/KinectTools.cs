using Microsoft.Kinect;
using Microsoft.Kinect.Face;

namespace WS2
{
    public partial class KinectControl
    {
        #region Variable 

        /// <summary>
        ///     Define the size of the camera
        /// </summary>
        public Rect DepthFrameSize { get; private set; }

        /// <summary>
        ///     Number of bodies tracked
        /// </summary>
        private int _bodyCount;

        private Body[] _bodies;
        private KinectSensor _sensor;

        /// <summary>
        ///     Reader for body frames
        /// </summary>
        private BodyFrameReader _bodyFrameReader;

        /// <summary>
        ///     Face frame sources
        /// </summary>
        private FaceFrameSource[] _faceFrameSources;

        /// <summary>
        ///     Face frame readers
        /// </summary>
        private FaceFrameReader[] _faceFrameReaders;

        /// <summary>
        ///     Storage for face frame results
        /// </summary>
        private FaceFrameResult[] _faceFrameResults;



        // specify the required face frame results
        private const FaceFrameFeatures FaceFrameFeatures = 
            Microsoft.Kinect.Face.FaceFrameFeatures.BoundingBoxInColorSpace 
            | Microsoft.Kinect.Face.FaceFrameFeatures.PointsInColorSpace 
            | Microsoft.Kinect.Face.FaceFrameFeatures.RotationOrientation 
            | Microsoft.Kinect.Face.FaceFrameFeatures.FaceEngagement 
            | Microsoft.Kinect.Face.FaceFrameFeatures.Glasses 
            | Microsoft.Kinect.Face.FaceFrameFeatures.Happy 
            | Microsoft.Kinect.Face.FaceFrameFeatures.LeftEyeClosed 
            | Microsoft.Kinect.Face.FaceFrameFeatures.RightEyeClosed 
            | Microsoft.Kinect.Face.FaceFrameFeatures.LookingAway 
            | Microsoft.Kinect.Face.FaceFrameFeatures.MouthMoved 
            | Microsoft.Kinect.Face.FaceFrameFeatures.MouthOpen;

        #endregion


        #region Gestion of Sensor and Reader

        /// <summary>
        ///     Get an instance of Sensor
        /// </summary>
        public void GetSensor()
        {
            _sensor = KinectSensor.GetDefault();
            _sensor.Open();
            DepthFrameSize = new Rect
            {
                Width = _sensor.DepthFrameSource.FrameDescription.Width,
                Height = _sensor.DepthFrameSource.FrameDescription.Height
            };
        }

        /// <summary>
        ///     Create a Reader for the data Recieve by Kinect
        /// </summary>
        public void OpenReader()
        {
            // open the reader for the body frames
            _bodyFrameReader = _sensor.BodyFrameSource.OpenReader();

            // wire handler for body frame arrival
            _bodyFrameReader.FrameArrived += Reader_BodyFrameArrived;

            // set the maximum number of bodies that would be tracked by Kinect
            _bodyCount = _sensor.BodyFrameSource.BodyCount;

            // allocate storage to store body objects
            _bodies = new Body[_bodyCount];

            // create a face frame source + reader to track each face in the FOV
            _faceFrameSources = new FaceFrameSource[_bodyCount];
            _faceFrameReaders = new FaceFrameReader[_bodyCount];
            for (var i = 0; i < _bodyCount; i++)
            {
                // create the face frame source with the required face frame features and an initial tracking Id of 0
                _faceFrameSources[i] = new FaceFrameSource(_sensor, 0, FaceFrameFeatures);

                // open the corresponding reader
                _faceFrameReaders[i] = _faceFrameSources[i].OpenReader();
            }

            // allocate storage to store face frame results for each face in the FOV
            _faceFrameResults = new FaceFrameResult[_bodyCount];

            for (var i = 0; i < _bodyCount; i++)
                if (_faceFrameReaders[i] != null)
                    _faceFrameReaders[i].FrameArrived += Reader_FaceFrameArrived;

            // open the sensor
            _sensor.Open();
        }


        /// <summary>
        ///     Close the Reader an clean the object
        /// </summary>
        public void CloseReader()
        {
            //this._faceFrameReader.FrameArrived -= Reader_FaceFrameArrived;

            for (var i = 0; i < _bodyCount; i++)
            {
                if (_faceFrameReaders[i] != null)
                {
                    // FaceFrameReader is IDisposable
                    _faceFrameReaders[i].Dispose();
                    _faceFrameReaders[i] = null;
                }

                if (_faceFrameSources[i] != null)
                {
                    // FaceFrameSource is IDisposable
                    _faceFrameSources[i].Dispose();
                    _faceFrameSources[i] = null;
                }
            }
        }

        /// <summary>
        ///     Release Sensor
        /// </summary>
        public void ReleaseSensor()
        {
            _sensor.Close();
            _sensor = null;
        }

        #endregion


        #region Get the Data of Kinect

        /// <summary>
        ///     Handles the body frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (var bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    // update body data
                    bodyFrame.GetAndRefreshBodyData(_bodies);

                    // draw the dark background
                    //dc.DrawRectangle(Brushes.Black, null, this.displayRect);

                    var drawFaceResult = false;

                    // iterate through each face source
                    for (var i = 0; i < _bodyCount; i++)
                        // check if a valid face is tracked in this face source
                        if (_faceFrameSources[i] != null && _faceFrameSources[i].IsTrackingIdValid)
                        {
                            // check if we have valid face frame results
                            if (_faceFrameResults[i] != null)
                            {
                                // draw face frame results
                                GetDataFaceFrameResults(_faceFrameResults[i]);

                                if (!drawFaceResult) drawFaceResult = true;
                            }
                        }
                        else
                        {
                            // check if the corresponding body is tracked 
                            if (_bodies[i].IsTracked) _faceFrameSources[i].TrackingId = _bodies[i].TrackingId;
                        }
                }
            }
        }

        /// <summary>
        ///     Handles the face frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_FaceFrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            using (var faceFrame = e.FrameReference.AcquireFrame())
            {
                if (faceFrame != null)
                {
                    // get the index of the face source from the face source array
                    var index = GetFaceSourceIndex(faceFrame.FaceFrameSource);

                    // check if this face frame has valid face frame results
                    if (ValidateFaceBoxAndPoints(faceFrame.FaceFrameResult))
                        _faceFrameResults[index] = faceFrame.FaceFrameResult;
                    else
                        _faceFrameResults[index] = null;
                }
            }
        }

        #endregion


        #region  Kinect Fonctions General

        /// <summary>
        ///     Returns the index of the face frame source
        /// </summary>
        /// <param name="faceFrameSource">the face frame source</param>
        /// <returns>the index of the face source in the face source array</returns>
        private int GetFaceSourceIndex(FaceFrameSource faceFrameSource)
        {
            var index = -1;

            for (var i = 0; i < _bodyCount; i++)
                if (_faceFrameSources[i] == faceFrameSource)
                {
                    index = i;
                    break;
                }

            return index;
        }


        /// <summary>
        ///     Validates face bounding box and face points to be within screen space
        /// </summary>
        /// <param name="faceResult">the face frame result containing face box and points</param>
        /// <returns>success or failure</returns>
        private static bool ValidateFaceBoxAndPoints(FaceFrameResult faceResult)
        {
            var isFaceValid = faceResult != null;

            if (isFaceValid)
            {
                var faceBox = faceResult.FaceBoundingBoxInColorSpace;
                if (faceBox != null)
                {
                    // check if we have a valid rectangle within the bounds of the screen space
                    isFaceValid = faceBox.Right - faceBox.Left > 0 &&
                                  faceBox.Bottom - faceBox.Top > 0;

                    if (isFaceValid)
                    {
                        var facePoints = faceResult.FacePointsInColorSpace;
                        if (facePoints != null)
                            foreach (var pointF in facePoints.Values)
                            {
                                // check if we have a valid face point within the bounds of the screen space
                                var isFacePointValid = pointF.X > 0.0f &&
                                                       pointF.Y > 0.0f;

                                if (!isFacePointValid)
                                {
                                    isFaceValid = false;
                                    break;
                                }
                            }
                    }
                }
            }

            return isFaceValid;
        }

        #endregion
    }


    public class Rect
    {
        public int Width;
        public int Height;
    }
}